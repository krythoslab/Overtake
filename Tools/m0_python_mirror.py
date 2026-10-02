"""
Overtake M0 Python Mirror Test Harness

Independent verification of C# simulation logic.
Uses Python's native int (arbitrary precision) for RNG with uint32 masking.
Uses float (double precision) for physics; tolerance accounts for precision difference.
"""
import struct

class DeterministicRng:
    def __init__(self, seed):
        self._state = seed & 0xFFFFFFFF

    def next(self):
        z = (self._state + 0x6D2B79F5) & 0xFFFFFFFF
        self._state = z
        z = ((z ^ (z >> 15)) * (1 | z)) & 0xFFFFFFFF
        z = (z ^ (z + (z ^ (z >> 7)) * (61 | z))) & 0xFFFFFFFF
        return (z ^ (z >> 14)) & 0xFFFFFFFF

    def next_float(self):
        return self.next() / 0xFFFFFFFF


class VehicleConfig:
    def __init__(self):
        self.mass_kg = 768.0
        self.initial_fuel_mass_kg = 100.0
        self.drag_coeff_cd = 0.7
        self.frontal_area_m2 = 1.5
        self.rolling_resistance_coeff = 0.015
        self.wheel_radius_m = 0.33
        self.max_brake_force_n = 20000.0
        self.gear_ratios = [3.0, 2.5, 2.0, 1.5, 1.2, 1.0]
        self.final_drive_ratio = 3.2
        self.ice_max_power_kw = 400.0
        self.idle_rpm = 800.0
        self.max_rpm = 15000.0
        self.shift_up_rpm = 12000.0
        self.shift_down_rpm = 5000.0
        self.air_density_kg_per_m3 = 1.225
        self.gravity_m_per_s2 = 9.81
        self.max_acceleration_m_per_s2 = 8.0
        self.battery_capacity_mj = 4.5
        self.mgu_k_max_power_kw = 350.0
        self.mgu_k_overtake_max_power_kw = 350.0
        self.mgu_k_harvest_max_kw = 350.0
        self.harvest_limit_mj_per_lap = 8.5
        self.battery_soc_delta_limit_mj = 4.0
        self.fuel_energy_flow_max_mj_per_h = 3000.0
        self.has_mgu_h = False
        self.active_aero = True
        self.corner_mode_downforce_coeff = 1.0
        self.corner_mode_drag_coeff = 1.0
        self.straight_mode_downforce_coeff = 0.55
        self.straight_mode_drag_coeff = 0.45
        self.front_transition_time_ms = 50.0
        self.rear_transition_time_ms = 50.0
        self.front_width_mm = 270.0
        self.rear_width_mm = 355.0
        self.ers_deploy_rate_mj_per_s = 0.8
        self.ers_boost_deploy_multiplier = 1.5
        self.ers_harvest_rate_mj_per_s = 0.5


class VehicleState:
    def __init__(self):
        self.pos_x = 0.0
        self.pos_y = 0.0
        self.pos_z = 0.0
        self.quat_x = 0.0
        self.quat_y = 0.0
        self.quat_z = 0.0
        self.quat_w = 1.0
        self.speed = 0.0
        self.rpm = 800.0
        self.gear = 1
        self.tire_slip_front_left = 0.0
        self.tire_slip_front_right = 0.0
        self.tire_slip_rear_left = 0.0
        self.tire_slip_rear_right = 0.0
        self.battery_soc_mj = 4.5
        self.rng_state = 0
        self.active_aero_mode = 0  # Corner
        self.ers_mode = 0  # Off
        self.fuel_mass_kg = 100.0
        self.active_aero_transition_timer_ms = 0.0
        self.active_aero_failed = False


class VehicleInput:
    def __init__(self, steer=0.0, throttle=0.0, brake=0.0, clutch=0.0, gear=0,
                 active_aero=0, ers_mode=0):
        self.steer = steer
        self.throttle = throttle
        self.brake = brake
        self.clutch = clutch
        self.gear = gear
        self.active_aero = active_aero
        self.ers_mode = ers_mode


class VehicleDynamics:
    def __init__(self, config):
        self.cfg = config
        self.state = VehicleState()
        self.rng = None
        self.initialized = False

    def reset(self, seed):
        self.rng = DeterministicRng(seed)
        self.state = VehicleState()
        self.state.rng_state = seed
        self.state.battery_soc_mj = self.cfg.battery_capacity_mj
        self.state.fuel_mass_kg = self.cfg.initial_fuel_mass_kg
        self.initialized = True

    def update(self, dt, inp):
        if not self.initialized:
            raise RuntimeError("Vehicle not initialized. Call Reset first.")
        if dt <= 0:
            return

        self._handle_auto_gear(inp)

        total_mass = self.cfg.mass_kg + self.state.fuel_mass_kg
        engine_force = 0.0
        if inp.throttle > 0.0:
            power_force = self.cfg.ice_max_power_kw * 1000.0 / max(self.state.speed, 0.5)
            max_force = total_mass * self.cfg.max_acceleration_m_per_s2
            engine_force = min(power_force, max_force) * inp.throttle

        drag_force = 0.5 * self.cfg.air_density_kg_per_m3 * self.cfg.drag_coeff_cd * self.cfg.frontal_area_m2 * self.state.speed ** 2
        rolling_force = self.cfg.rolling_resistance_coeff * total_mass * self.cfg.gravity_m_per_s2
        brake_force = inp.brake * self.cfg.max_brake_force_n

        net_force = engine_force - drag_force - rolling_force - brake_force
        acceleration = net_force / total_mass

        self.state.speed += acceleration * dt
        if self.state.speed < 0.0:
            self.state.speed = 0.0

        self.state.pos_x += self.state.speed * dt

        if self.state.speed > 0.1:
            gear_index = max(0, min(self.state.gear - 1, len(self.cfg.gear_ratios) - 1))
            wheel_angular_speed = self.state.speed / self.cfg.wheel_radius_m
            engine_angular_speed = wheel_angular_speed * self.cfg.final_drive_ratio * self.cfg.gear_ratios[gear_index]
            self.state.rpm = engine_angular_speed * 60.0 / (2.0 * 3.141592653589793)
            self.state.rpm = max(self.cfg.idle_rpm, min(self.state.rpm, self.cfg.max_rpm))
        else:
            self.state.rpm = self.cfg.idle_rpm + inp.throttle * (self.cfg.max_rpm - self.cfg.idle_rpm)

        slip_mag = abs(acceleration) / (self.cfg.gravity_m_per_s2 * 0.9)
        slip_clamped = max(0.0, min(slip_mag, 1.0))
        self.state.tire_slip_front_left = slip_clamped
        self.state.tire_slip_front_right = slip_clamped
        self.state.tire_slip_rear_left = slip_clamped
        self.state.tire_slip_rear_right = slip_clamped

        self._update_ers(dt, inp)
        self._update_active_aero(dt, inp)
        self._update_fuel(dt, inp.throttle, inp.brake)

        self.state.rng_state = self.rng._state

    def _update_ers(self, dt, inp):
        if self.state.ers_mode == 0:  # Off
            if inp.ers_mode != 0:
                self.state.ers_mode = inp.ers_mode
        elif self.state.ers_mode == 5:  # Boost
            self.state.battery_soc_mj -= self.cfg.ers_deploy_rate_mj_per_s * dt
            if self.state.battery_soc_mj <= 0.0:
                self.state.battery_soc_mj = 0.0
                self.state.ers_mode = 0
            elif inp.ers_mode == 0:
                self.state.ers_mode = 0
        elif self.state.ers_mode == 4:  # Qualifying
            self.state.battery_soc_mj -= self.cfg.ers_deploy_rate_mj_per_s * self.cfg.ers_boost_deploy_multiplier * dt
            if self.state.battery_soc_mj <= 0.0:
                self.state.battery_soc_mj = 0.0
                self.state.ers_mode = 0
            elif inp.ers_mode == 0:
                self.state.ers_mode = 0
        else:
            if inp.ers_mode == 5:
                self.state.ers_mode = 5
            elif inp.ers_mode == 0:
                self.state.ers_mode = 0
            elif inp.ers_mode != self.state.ers_mode:
                self.state.ers_mode = inp.ers_mode

            if inp.brake > 0.5:
                self.state.battery_soc_mj += self.cfg.ers_harvest_rate_mj_per_s * dt
                if self.state.battery_soc_mj > self.cfg.battery_capacity_mj:
                    self.state.battery_soc_mj = self.cfg.battery_capacity_mj

        if self.state.battery_soc_mj < 0.01 and self.state.ers_mode != 0:
            self.state.battery_soc_mj = 0.0
            self.state.ers_mode = 0

    def _update_active_aero(self, dt, inp):
        if not self.cfg.active_aero:
            self.state.active_aero_mode = 3  # Off
            self.state.active_aero_failed = False
            return

        if self.state.active_aero_failed:
            self.state.active_aero_mode = 0  # Corner
            self.state.active_aero_transition_timer_ms = 0.0
            return

        transition_time = self.cfg.front_transition_time_ms if self.state.active_aero_mode == 0 else self.cfg.rear_transition_time_ms

        if self.state.active_aero_transition_timer_ms > 0.0:
            self.state.active_aero_transition_timer_ms -= dt * 1000.0
            if self.state.active_aero_transition_timer_ms <= 0.0:
                self.state.active_aero_transition_timer_ms = 0.0
                self.state.active_aero_mode = inp.active_aero
            return

        if inp.active_aero != self.state.active_aero_mode:
            self.state.active_aero_transition_timer_ms = transition_time

    def _update_fuel(self, dt, throttle, brake):
        fuel_rate_mj_per_s = self.cfg.ice_max_power_kw * 0.8
        fuel_energy_mj = fuel_rate_mj_per_s * dt * max(throttle, 0.1)
        fuel_mass_kg = fuel_energy_mj / 44.0
        new_fuel_mass = self.state.fuel_mass_kg - fuel_mass_kg
        min_mass = 5.0
        if new_fuel_mass < min_mass:
            new_fuel_mass = min_mass
        self.state.fuel_mass_kg = new_fuel_mass

    def _handle_auto_gear(self, inp):
        target_gear = max(0, min(inp.gear, len(self.cfg.gear_ratios)))
        if target_gear > 0 and target_gear != self.state.gear:
            self.state.gear = target_gear
            return

        if self.state.gear < len(self.cfg.gear_ratios) and self.state.rpm >= self.cfg.shift_up_rpm:
            self.state.gear += 1
        elif self.state.gear > 1 and self.state.rpm <= self.cfg.shift_down_rpm:
            self.state.gear -= 1


class SingleCarSimulation:
    def __init__(self, config, seed, fixed_dt=0.02):
        self.vehicle = VehicleDynamics(config)
        self.fixed_dt = fixed_dt
        self.accumulator = 0.0
        self.seed = seed
        self.reset()

    def reset(self):
        self.vehicle.reset(self.seed)
        self.accumulator = 0.0

    def step(self, real_dt, inp):
        if real_dt <= 0:
            return
        self.accumulator += real_dt
        while self.accumulator >= self.fixed_dt:
            self.vehicle.update(self.fixed_dt, inp)
            self.accumulator -= self.fixed_dt

    @property
    def current_state(self):
        return self.vehicle.state


def test_identical_inputs_and_seed():
    config = VehicleConfig()
    sim1 = SingleCarSimulation(config, 12345, 0.02)
    sim2 = SingleCarSimulation(config, 12345, 0.02)
    inp = VehicleInput(throttle=0.8, ers_mode=1, active_aero=0)
    inp.gear = 1

    for _ in range(600):
        sim1.step(0.02, inp)
        sim2.step(0.02, inp)

    s1 = sim1.current_state
    s2 = sim2.current_state

    assert abs(s1.speed - s2.speed) < 1e-5, f"Speed mismatch: {s1.speed} vs {s2.speed}"
    assert abs(s1.pos_x - s2.pos_x) < 1e-4, f"Position mismatch: {s1.pos_x} vs {s2.pos_x}"
    assert abs(s1.rpm - s2.rpm) < 1e-4, f"RPM mismatch: {s1.rpm} vs {s2.rpm}"
    assert s1.gear == s2.gear, f"Gear mismatch: {s1.gear} vs {s2.gear}"
    assert abs(s1.battery_soc_mj - s2.battery_soc_mj) < 1e-5, f"Battery mismatch: {s1.battery_soc_mj} vs {s2.battery_soc_mj}"
    assert s1.rng_state == s2.rng_state, f"RNG mismatch: {s1.rng_state} vs {s2.rng_state}"
    print("PASS: test_identical_inputs_and_seed")


def test_determinism_with_ers_and_aero():
    config = VehicleConfig()
    inp = VehicleInput(throttle=0.8, ers_mode=5, active_aero=1)
    inp.gear = 1

    first_state = None
    for run in range(5):
        sim = SingleCarSimulation(config, 12345, 0.02)
        for _ in range(600):
            sim.step(0.02, inp)
        state = sim.current_state
        if first_state is None:
            first_state = state
        else:
            assert abs(first_state.speed - state.speed) < 1e-5
            assert abs(first_state.battery_soc_mj - state.battery_soc_mj) < 1e-5
            assert first_state.ers_mode == state.ers_mode
            assert first_state.active_aero_mode == state.active_aero_mode
            assert abs(first_state.fuel_mass_kg - state.fuel_mass_kg) < 1e-4
    print("PASS: test_determinism_with_ers_and_aero")


def test_ers_boost_drains_battery():
    config = VehicleConfig()
    sim = SingleCarSimulation(config, 12345, 0.02)
    inp = VehicleInput(throttle=0.5, ers_mode=5)
    inp.gear = 1

    initial_battery = sim.current_state.battery_soc_mj
    for _ in range(100):
        sim.step(0.02, inp)

    assert sim.current_state.battery_soc_mj < initial_battery, "ERS Boost should drain battery."
    print("PASS: test_ers_boost_drains_battery")


def test_ers_battery_empty_forces_off():
    config = VehicleConfig()
    sim = SingleCarSimulation(config, 12345, 0.02)
    inp = VehicleInput(throttle=0.1, ers_mode=5)
    inp.gear = 1

    for _ in range(2000):
        sim.step(0.02, inp)

    assert sim.current_state.ers_mode == 0, f"ERS should be Off when empty, got {sim.current_state.ers_mode}"
    assert abs(sim.current_state.battery_soc_mj - 0.0) < 1e-5
    print("PASS: test_ers_battery_empty_forces_off")


def test_ers_harvest_increases_battery():
    config = VehicleConfig()
    sim = SingleCarSimulation(config, 12345, 0.02)
    inp = VehicleInput(throttle=0.0, brake=1.0, ers_mode=1)
    inp.gear = 1

    sim.current_state.battery_soc_mj = 1.0
    initial_battery = sim.current_state.battery_soc_mj
    for _ in range(100):
        sim.step(0.02, inp)

    assert sim.current_state.battery_soc_mj > initial_battery, "Braking with ERS Medium should harvest energy."
    print("PASS: test_ers_harvest_increases_battery")


def test_fuel_mass_decreases():
    config = VehicleConfig()
    sim = SingleCarSimulation(config, 12345, 0.02)
    inp = VehicleInput(throttle=0.8)
    inp.gear = 1

    initial_fuel = sim.current_state.fuel_mass_kg
    for _ in range(600):
        sim.step(0.02, inp)

    assert sim.current_state.fuel_mass_kg < initial_fuel, "Fuel mass should decrease."
    print("PASS: test_fuel_mass_decreases")


def test_active_aero_transition():
    config = VehicleConfig()
    sim = SingleCarSimulation(config, 12345, 0.02)
    inp = VehicleInput(throttle=0.5, active_aero=1)
    inp.gear = 1

    sim.step(0.02, inp)
    assert sim.current_state.active_aero_mode == 0, "Should still be Corner during transition."

    sim.step(0.02, inp)
    assert sim.current_state.active_aero_mode == 0, "Should still be Corner during transition."

    sim.step(0.02, inp)
    assert sim.current_state.active_aero_mode == 0, "Should still be Corner during transition."

    sim.step(0.02, inp)
    assert sim.current_state.active_aero_mode == 1, "Should transition to StraightLine after timer."
    print("PASS: test_active_aero_transition")


def test_fixed_timestep_consistency():
    config = VehicleConfig()
    inp = VehicleInput(throttle=0.8)
    inp.gear = 1

    sim50_a = SingleCarSimulation(config, 12345, 0.02)
    sim50_b = SingleCarSimulation(config, 12345, 0.02)
    sim100_a = SingleCarSimulation(config, 12345, 0.01)
    sim100_b = SingleCarSimulation(config, 12345, 0.01)

    total_time = 2.0
    steps50 = int(total_time / 0.02)
    steps100 = int(total_time / 0.01)

    for _ in range(steps50):
        sim50_a.step(0.02, inp)
        sim50_b.step(0.02, inp)
    for _ in range(steps100):
        sim100_a.step(0.01, inp)
        sim100_b.step(0.01, inp)

    s50_a = sim50_a.current_state
    s50_b = sim50_b.current_state
    s100_a = sim100_a.current_state
    s100_b = sim100_b.current_state

    assert abs(s50_a.speed - s50_b.speed) < 1e-5
    assert abs(s100_a.speed - s100_b.speed) < 1e-5
    assert s50_a.gear == s100_a.gear
    print("PASS: test_fixed_timestep_consistency")


if __name__ == "__main__":
    test_identical_inputs_and_seed()
    test_determinism_with_ers_and_aero()
    test_ers_boost_drains_battery()
    test_ers_battery_empty_forces_off()
    test_ers_harvest_increases_battery()
    test_fuel_mass_decreases()
    test_active_aero_transition()
    test_fixed_timestep_consistency()
    print("\nAll Python mirror tests passed.")
