/datum/procedural_station/generator/section
	/// list of coords
	var/list/vertex_turfs
	var/list/all_turfs
	var/list/shared_border_turfs

	var/list/disposals_in
	var/turf/disposals_out

	var/list/powernet_connections

	var/list/atmos_distro_connections
	var/list/atmos_waste_connections

/datum/procedural_station/generator/section/New(datum/procedural_parameters/working/parameters, list/vertex_turfs, list/shared_border_turfs, list/disposals_in, turf/disposals_out, list/powernet_connections, list)
	..()

/datum/procedural_station/generator/section/proc/return_turfs()
	RETURN_TYPE(/list)

	if(all_turfs)
		return all_turfs

	var/list/vertex_turfs = src.vertex_turfs
	// optimization, 2 verticies? use block()
	if(vertex_turfs.len == 2)
		all_turfs = block(vertex_turfs[1], vertex_turfs[2])
		return all_turfs

	var/list/sorted_vertex_turfs = sortTim(vertex_turfs.Copy(), /proc/cmp_turf_coords)
