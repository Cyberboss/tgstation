/datum/procedural_station/controller
	/// Map of (stringified) rand() seeds to number of generate calls. Used to make reproducible maps
	var/datum/map_config/generated_map_config

/datum/procedural_station/controller/New()
	..(list())

/// Main proc, generates a station
/datum/procedural_station/controller/proc/generate_station(list/station_seed = null)
	var/from_scratch = !station_seed
	var/current_seed = rand()

	var/datum/procedural_station/generator/core/core_generator = new

	var/list/generator_queue = list(core_generator)
	var/seed_iterations = 0
	var/sleep_iterations = 0
	var/total_iterations = 0

	var/target_seed_iterations
	if(!from_scratch)
		var/seed_struct = parameters.seed_list["[sleep_iterations]"]
		current_seed = seed_struct["seed"]
		target_seed_iterations = seed_struct["iter"]

	rand_seed(current_seed)

	while (generator_queue.len)
		var/datum/procedural_station/generator/G = generator_queue[1]
		generator_queue.Cut(1, 2)

		testing("Running generator #[++total_iterations]: [G.type]...")
		G.generate()

		var/list/sub_generators = G.sub_generators
		if(length(sub_generators))
			generator_queue += sub_generators

		// some day in the future, someone will fuck up and start referencing the generators when they shouldn't be
		// just gonna leave this here
		// qdel(G)

		++seed_iterations

		var/break_time = from_scratch ? TICK_CHECK : seed_iterations == target_seed_iterations
		if(break_time)
			if(!from_scratch)
				parameters.seed_list["[sleep_iterations]"] = list("seed" = current_seed, "iter" = seed_iterations)
			stoplag()
			++sleep_iterations
			if(from_scratch)
				var/seed_struct = parameters.seed_list["[sleep_iterations]"]
				current_seed = seed_struct["seed"]
				target_seed_iterations = seed_struct["iter"]
			else
				current_seed = rand()
			rand_seed(current_seed)
			seed_iterations = 0

	generated_map_config = core_generator.get_map_config()
