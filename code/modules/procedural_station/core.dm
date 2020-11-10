/datum/procedural_station/generator/core/generate()
	// BACKDROP
	sub_generator(/datum/procedural_station/generator/backdrop/space)

	// LAYOUT
	sub_generator(/datum/procedural_station/generator/layout/rectangle)

/datum/procedural_station/generator/core/proc/get_map_config()
	RETURN_TYPE(/datum/map_config)
	CRASH("Unimplemented")
