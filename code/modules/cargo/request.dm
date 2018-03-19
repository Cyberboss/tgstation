#define REQUEST_STATE_PENDING 0
#define REQUEST_STATE_IN_REVIEW 1
#define REQUEST_STATE_APPROVED_EN_ROUTE 2
#define REQUEST_STATE_APPROVED_DELIVERED 3
#define REQUEST_STATE_APPROVED_DELIVERED_MANUAL 4
#define REQUEST_STATE_DENIED 5
#define REQUEST_STATE_UNKNOWN 6

GLOBAL_LIST_EMPTY(cargo_requests)

/datum/cargo_request
	var/id
	var/requester_name
	var/requester_ckey

	var/department
	//mules will wait around until someone take the cargo
	//mail deliveries won't actually be sent until the reciept is acknowledged
	var/received_by

	var/archived = FALSE
	var/state = REQUEST_STATE_PENDING

	var/from_cargo = FALSE
	var/high_priority = FALSE

	var/cost


	var/requested_amount
	var/delivered_amount

	var/list/log_messages

	var/requested_path
	var/datum/supply_pack/pack
	
	var/static/id_counter = rand(0, 9000)

/datum/cargo_request/New(mob/living/requester, department, from_cargo, datum/supply_pack/pack, reason)
	src.department = department
	src.from_cargo = from_cargo
	src.pack = pack
	id = ++id_counter
	requester_name = requester.name
	requester_ckey = requester.ckey

	log_messages = list("Request #[id] created by [requester_name]", "Reason: [reason]")

	if(!pack && !from_cargo)
		log_messages += list("Non-standard request: May require requisitions from other departments")

	LAZYADD(GLOB.cargo_requests[department], src)
	for(var/I in GLOB.consoles_by_department[department])
		var/obj/machinery/requests_console/R = I
		R.update_icon()

/datum/cargo_request/Destroy()
	LAZYREMOVE(GLOB.cargo_requests[department], src)
	return ..()

/datum/cargo_request/proc/CanBeOrderedFromCentcom()
	return pack ? TRUE : FALSE

/datum/cargo_request/proc/GenerateFromCentcom(turf/T)
	if(!pack)
		CRASH("GenerateFromCentcom called on non-standard cargo request")
	var/obj/structure/closet/crate/C = pack.generate(T)
	var/obj/item/paper/fluff/jobs/cargo/manifest/M = new(C, src)

	if(M.errors & MANIFEST_ERROR_ITEM)
		if(istype(C, /obj/structure/closet/crate/secure) || istype(C, /obj/structure/closet/crate/large))
			M.errors &= ~MANIFEST_ERROR_ITEM
		else
			var/lost = max(round(C.contents.len / 10), 1)
			while(--lost >= 0)
				qdel(pick(C.contents))
	return C

/datum/cargo_request/proc/generateRequisition(turf/T)
	var/obj/item/paper/P = new(T)

	P.name = "requisition form - #[id][pack ? " ([pack.name])" : ""]"
	var/list/info = list("<h2>[station_name()] Supply Requisition</h2><hr/>Order #[id]<br/>")
	var/obj/item/I = requested_path
	info += "Item: [pack ? pack.name : initial(I.name)]<br/>"
	if(pack)
		info += "Access Restrictions: [get_access_desc(pack.access)]<br/>"
	info += "Requested by: [orderer]<br/>Rank: [orderer_rank]<br/>Comment: [reason]<br/>"
	P.info = info.Join()

	P.update_icon()
	return P
