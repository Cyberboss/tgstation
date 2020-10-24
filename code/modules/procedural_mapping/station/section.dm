/datum/procedural_station/generator/station
	/// list of coords
	var/list/vertex_turfs
	var/list/shared_border_turfs

	var/list/disposals_in
	var/list/disposals_out

	var/list/powernet_connections

	var/list/atmos_distro_connections
	var/list/atmos_waste_connections

/datum/procedural_station/generator/section/proc/return_turfs()
	RETURN_TYPE(/list)
	CRASH("Unimplemented")
