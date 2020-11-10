/// Single generator module, can do anything
/datum/procedural_station/generator
	/// List of /datum/procedural_station/generators to run after this one
	var/list/sub_generators

/// Run generation step
/datum/procedural_station/generator/proc/generate()
	SHOULD_CALL_PARENT(FALSE)
	SHOULD_NOT_SLEEP(TRUE)
	CRASH("generate not implemented on [type]!")

/// Add a given path to sub_generators
/datum/procedural_station/generator/proc/sub_generator(path)
	if(!ispath(path, /datum/procedural_station/generator))
		CRASH("Non-generator passed to sub_generator!")

	LAZYADD(sub_generators, new path(parameters))
