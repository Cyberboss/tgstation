GLOBAL_LIST_EMPTY(allConsoles)
GLOBAL_LIST_EMPTY(consoles_by_department)

#define EMERGENCY_SECURITY 1
#define EMERGENCY_MEDICAL 2
#define EMERGENCY_ENGINEERING 3

/obj/machinery/requests_console
	name = "requests console"
	desc = "A console intended to send requests to different departments on the station."
	anchored = TRUE
	icon = 'icons/obj/terminals.dmi'
	icon_state = "req_comp0"
	max_integrity = 300
	armor = list("melee" = 70, "bullet" = 30, "laser" = 30, "energy" = 30, "bomb" = 0, "bio" = 0, "rad" = 0, "fire" = 90, "acid" = 90)

	var/hacked = FALSE
	var/open = FALSE
	var/receive_ore_updates = FALSE

	var/announcementConsole
	var/department

	var/msgStamped
	var/emergencyType

	var/list/cargo_requests

	var/obj/item/stamp/stamper
	var/obj/item/device/radio/radio

/obj/machinery/requests_console/Initialize()
	. = ..()
	radio = new
	radio.listening = FALSE
	GLOB.allConsoles += src
	LAZYADD(GLOB.consoles_by_department[department], src)

/obj/machinery/requests_console/Destroy()
	QDEL_NULL(radio)
	LAZYREMOVE(GLOB.consoles_by_department[department], src)
	if(!GLOB.consoles_by_department[department])
		for(var/I in GLOB.cargo_requests[department])
			var/datum/cargo_request/C = I
			C.state = REQUEST_STATE_UNKNOWN
	GLOB.allConsoles -= src
	return ..()

/obj/machinery/requests_console/power_change()
	..()
	update_icon()

/obj/machinery/requests_console/update_icon()
	if(stat & NOPOWER)
		set_light(0)
	else
		set_light(1.4,0.7,"#34D352")//green light
	if(open)
		if(!hacked)
			icon_state="req_comp_open"
		else
			icon_state="req_comp_rewired"
	else if(stat & NOPOWER)
		if(icon_state != "req_comp_off")
			icon_state = "req_comp_off"
	else
		var/list/cargo_requests = GLOB.cargo_requests[department]
		if(emergencyType)
			icon_state = "req_comp3"
		else if(LAZYLEN(cargo_requests))
			icon_state = "req_comp1"
			for(var/I in cargo_requests)
				var/datum/cargo_request/C = I
				if(C.high_priority && C.from_cargo && C.state < REQUEST_STATE_APPROVED_DELIVERED)
					icon_state = "req_comp2"
				break
		else
			icon_state = "req_comp0"

/obj/machinery/requests_console/say_mod(input, message_mode)
	var/ending = copytext(input, length(input) - 2)
	if (ending == "!!!")
		return "blares"
	return ..()

/obj/machinery/requests_console/crowbar_act(mob/living/user, obj/item/I)
	if(open)
		to_chat(user, "<span class='notice'>You close the maintenance panel.</span>")
		open = FALSE
	else
		to_chat(user, "<span class='notice'>You open the maintenance panel.</span>")
		open = TRUE
	update_icon()

/obj/machinery/requests_console/screwdriver_act(mob/living/user, obj/item/I)
	if(open)
		hacked = !hacked
		if(hacked)
			to_chat(user, "<span class='notice'>You modify the wiring.</span>")
		else
			to_chat(user, "<span class='notice'>You reset the wiring.</span>")
		update_icon()
	else
		to_chat(user, "<span class='warning'>You must open the maintenance panel first!</span>")

/obj/machinery/requests_console/attackby(obj/item/O, mob/living/user, params)
	if (istype(O, /obj/item/stamp))
		stamper = O
		ui_interact(user)
		return
	return ..()

/*
Request console v2 by Cyberboss

Request consoles are now a means for commmnicating mainly with cargo

Main Menu:
	View Requests
	Request Supplies

	Security Emergency
	Medical Emergency
	Engineering Emergency

	Make station announcement - Head consoles only

View Requests:
	Messages from cargo requesting supplies. Cargo can specify requests will be auto rejected until these are fufilled

Request supplies:
	History

	List of available cargo crates + list of individual items
	Requests can be marked as pending, in review, approved, or denied
	The request will be cleared when cargo marks it as delivered manually (noted on the request) or its sent via the automated delivery system

Emergency buttons:
	Only one can be pushed at a time and the user cannot clear it. This will cause the automated announcement system to repeat the emergency and location every minute to that department's radio as well as their request consoles to flash. It can be cleared via the department consoles or via the console that initiated it only by those that have access to the department that help was requested from 

*/

/obj/machinery/requests_console/ui_interact(mob/user, ui_key = "main", datum/tgui/ui, force_open = FALSE, datum/tgui/master_ui, datum/ui_state/state = GLOB.default_state) // Remember to use the appropriate state.
	ui = SStgui.try_update_ui(user, src, ui_key, ui, force_open)
	if(!ui)
		ui = new(user, src, ui_key, "requests_console", name, 300, 300, master_ui, state)
		ui.set_autoupdate(FALSE)
		ui.open()

/obj/machinery/requests_console/ui_data(mob/user)
	. = list()

	stamper = null
  
/obj/machinery/requests_console/ui_act(action, params)
	if(..())
		return
