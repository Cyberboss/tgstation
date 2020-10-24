/datum/procedural_station/generator
	var/list/sub_generators = list()

/datum/procedural_station/generator/proc/generate()
	SHOULD_CALL_PARENT(FALSE)
	SHOULD_NOT_SLEEP(TRUE)
	CRASH("generate not implemented on [type]!")

/datum/procedural_station/generator/proc/sub_generator(path)
	sub_generators += new path(parameters)
