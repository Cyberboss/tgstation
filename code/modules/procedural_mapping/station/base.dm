/// Base /datum for all procedural_station generation
/datum/procedural_station
	var/datum/procedural_station/working/parameters

/datum/procedural_station/New(datum/procedural_station/working/parameters)
	if(!parameters)
		CRASH("[type] missing parameters reference!")

	src.parameters = parameters

/**
  * Ensure a given coordinate exists
  *
  * * x - The x coordinate
  * * y - The y coordinate
  * * z - The z coordinate
  */
/datum/procedural_station/proc/ensure_coord(x, y, z)
	RETURN_TYPE(/list)
	var/access_string = "[z]"
	var/list/z_level = parameters.prototype_coords[access_string]
	if(!z_level)
		testing("Creating z level [access_string]...")
		z_level = list()
		parameters.prototype_coords[access_string] = z_level

	access_string = "[y]"
	var/list/y_level = z_level[access_string]
	if(!y_level)
		testing("")
		y_level = list()
		z_level[access_string] = y_level

	access_string = "[x]"
	var/list/x_level = y_level[access_string]
	if(!x_level)
		// reserve 2 spots, 1 for area, other for turf
		x_level = list(null, null)
		y_level[access_string] = x_level

	return x_level

/**
  * Set a prototype at a given coordinate
  *
  * * x - The x coordinate
  * * y - The y coordinate
  * * z - The z coordinate
  * * prototype - The /atom path
  * * path_filter - The paths to remove
  * * var_edits - var edits on [prototype]
  */
/datum/procedural_station/proc/set_single_prototype(x, y, z, prototype, path_filter, list/var_edits = null)
	var/prototype_list = ensure_coord(x, y, z)
	for(var/path in prototype_list)
		if(ispath(path, path_filter))
			prototype_list -= path
	prototype_list[prototype] = var_edits

/**
  * Set the /area at a given coordinate
  *
  * * x - The x coordinate
  * * y - The y coordinate
  * * z - The z coordinate
  * * prototype - The /area path
  * * var_edits - var edits on [prototype]
  */
/datum/procedural_station/proc/set_area(x, y, z, prototype, list/var_edits = null)
	if(!ispath(prototype, /area))
		CRASH("Non-/area path passed to set_area!")
	var/prototype_list = ensure_coord(x, y, z)
	set_single_prototype(x, y, z, prototype, /area, var_edits)

/**
  * Set the /turf at a given coordinate
  *
  * * x - The x coordinate
  * * y - The y coordinate
  * * z - The z coordinate
  * * prototype - The /turf path
  * * var_edits - var edits on [prototype]
  */
/datum/procedural_station/proc/set_turf(x, y, z, prototype, list/var_edits = null)
	if(!ispath(prototype, /turf))
		CRASH("Non-/turf path passed to set_area!")
	set_single_prototype(x, y, z, prototype, /turf, var_edits)

/**
  * Add an /obj at a given coordinate
  *
  * * x - The x coordinate
  * * y - The y coordinate
  * * z - The z coordinate
  * * prototype - The atom path
  * * var_edits - var edits on [prototype]
  */
/datum/procedural_station/proc/add_obj(x, y, z, prototype, list/var_edits)
	if(!ispath(prototype, /obj))
		CRASH("Non-/obj path passed to add_obj!")

	var/prototype_list = ensure_coord(x, y, z)
	var/existing_prototype_list = prototype_list[prototype]
	if(!existing_prototype_list)
		existing_prototype_list = list()
	existing_prototype_list += list(var_edits)
