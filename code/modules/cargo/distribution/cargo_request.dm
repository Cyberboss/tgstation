#define REQUEST_STATE_PENDING 0
#define REQUEST_STATE_IN_REVIEW 1
#define REQUEST_STATE_APPROVED_EN_ROUTE 2
#define REQUEST_STATE_APPROVED_DELIVERED 3
#define REQUEST_STATE_APPROVED_DELIVERED_MANUAL 4
#define REQUEST_STATE_DENIED 5
#define REQUEST_STATE_UNKNOWN 6

GLOBAL_LIST_EMPTY(cargo_requests)

/datum/cargo_request
	var/requester_name
	var/department
	//mules will wait around until someone take the cargo
	//mail deliveries won't actually be sent until the reciept is acknowledged
	var/received_by

	var/archived = FALSE
	var/state = REQUEST_STATE_PENDING

	var/from_cargo = FALSE
	var/high_priority = FALSE

	var/requested_crate
	var/requested_path

	var/requested_amount
	var/delivered_amount

	var/list/log_messages

/datum/cargo_request/New(mob/living/requester, department, from_cargo = FALSE)
	src.department = department
	src.from_cargo = from_cargo

	LAZYADD(GLOB.cargo_requests[department], src)
	for(var/I in GLOB.consoles_by_department[department])
		var/obj/machinery/requests_console/R = I
		R.update_icon()

/datum/cargo_request/Destroy()
	LAZYREMOVE(GLOB.cargo_requests[department], src)
	return ..()