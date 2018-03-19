/obj/item/paper/fluff/jobs/cargo/manifest
	var/errors = NONE
	var/obj/structure/closet/crate/crate

/obj/item/paper/fluff/jobs/cargo/manifest/Initialize(mapload, datum/cargo_request/request)
	. = ..()

	if(prob(MANIFEST_ERROR_CHANCE))
		errors |= MANIFEST_ERROR_NAME
	if(prob(MANIFEST_ERROR_CHANCE))
		errors |= MANIFEST_ERROR_CONTENTS
	if(prob(MANIFEST_ERROR_CHANCE))
		errors |= MANIFEST_ERROR_ITEM

	var/station_name = (errors & MANIFEST_ERROR_NAME) ? new_station_name() : station_name()

	name = "shipping manifest - #[id] ([pack.name])"
	var/list/info = list("<h2>[command_name()] Shipping Manifest</h2><hr/>Order #[id]<br/>Destination: [station_name]<br/>Item: [pack.name]<br/>Contents: <br/><ul>")
	for(var/AM in loc.contents - src)
		var/itemLine = "<li>[AM]</li>"
		if((errors & MANIFEST_ERROR_CONTENTS))
			if(prob(50))
				info += itemLine
			else
				continue
		info += itemLine
	info += "</ul><h4>Stamp below to confirm receipt of goods:</h4>"

	update_icon()
	crate = C
	crate.manifest = P
	crate.update_icon()

/obj/item/paper/fluff/jobs/cargo/manifest/Destroy()
	if(crate)
		crate.manifest = null
		crate = null
	return ..()

/obj/item/paper/fluff/jobs/cargo/manifest/proc/is_approved()
	return stamped && stamped.len && !is_denied()

/obj/item/paper/fluff/jobs/cargo/manifest/proc/is_denied()
	return stamped && ("stamp-deny" in stamped)
