/datum/procedural_station/generator/core/generate()
	if(prob(100))
		sub_generator(/datum/procedural_station/generator/single_z)
	else
		sub_generator(/datum/procedural_station/generator/multi_z)

/datum/procedural_station/generator/core/proc/get_map_config()
	RETURN_TYPE(/datum/map_config)
	CRASH("Unimplemented")
