/datum/procedural_parameters
	var/station_size = PROCEDURAL_STATION_SIZE_MED

/datum/procedural_parameters/working
	/// Master prototype reference list, structure with strigified coords "[z]" -> "[y]" -> "[x]"
	var/list/prototype_coords = list()
