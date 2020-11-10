/datum/procedural_parameters
	var/station_size = PS_SIZE_MED
	var/list/seed_list

/datum/procedural_parameters/working
	/// Master prototype reference list, structure with strigified coords "[z]" -> "[y]" -> "[x]"
	var/list/prototype_coords = list()
