/obj/machinery/door/poddoor/preopen/lobby
	name = "Crew Boarding Room"

/obj/machinery/door/poddoor/preopen/lobby/Initialize()
	. = ..()
	SSticker.lobby.shutters += src

/obj/machinery/door/poddoor/preopen/lobby/Destroy()
	SSticker.lobby.shutters -= src
	return ..()

/obj/structure/lobby_teleporter
	name = "To Arrivals Shuttle"
	icon = 'icons/obj/machines/teleporter.dmi'
	icon_state = "tele1"

/obj/structure/lobby_teleporter/CollidedWith(mob/living/carbon/human/lobby/player)
	if(istype(player))
		player.AttemptJoin()
		return

	//garbage
	do_sparks(5, FALSE, src)
	qdel(player)

/turf/open/floor/light/lobby
	name = "Crew Boarding Room"
	coloredlights = list("b")
	can_modify_colour = FALSE
	var/timer_id

/turf/open/floor/light/lobby/Initialize()
	. = ..()
	SSticker.lobby.lights += src

/turf/open/floor/light/lobby/Destroy()
	SSticker.lobby.lights -= src
	return ..()

/turf/open/floor/light/lobby/proc/WarningSequence()
	coloredlights = list("r", "g")
	ToggleColour()

/turf/open/floor/light/lobby/proc/ToggleColour()
	currentcolor = currentcolor == 1 ? 2 : 1
	update_icon()
	timer_id = addtimer(CALLBACK(src, .proc/ToggleColour), 5, TIMER_CLIENT_TIME | TIMER_STOPPABLE)

/turf/open/floor/light/lobby/proc/Normalize()
	deltimer(timer_id)
	coloredlights = list("b")
	currentcolor = 1
	update_icon()

/obj/machinery/computer/lobby/setup_character
	name = "Setup Character"
	icon_screen = "comm_monitor"

/obj/machinery/computer/lobby/setup_character/attack_hand(mob/player)
	player.client.prefs.ShowChoices(player)

/obj/machinery/computer/lobby/observer
	name = "Become Observer"
	icon_screen = "comm_monitor"

/obj/machinery/computer/lobby/observer/attack_hand(mob/living/carbon/human/lobby/player)
	player.make_me_an_observer()

/obj/machinery/computer/lobby/poll
	name = "Show Player Polls"
	icon_screen = "syndishuttle"
	var/image/new_notification

/obj/machinery/computer/lobby/poll/Initialize()
	. = ..()
	SSticker.lobby.poll_computers += src
	new_notification = image('icons/mob/screen_gen.dmi', loc, "new_arrow")
	var/matrix/shift_up = matrix(new_notification.transform)
	shift_up.Translate(0, 20)
	new_notification.transform = shift_up

/obj/machinery/computer/lobby/poll/Destroy()
	SSticker.lobby.poll_computers -= src
	return ..()

/obj/machinery/computer/lobby/poll/attack_hand(mob/living/carbon/human/lobby/player)
	player.handle_player_polling()

/obj/machinery/requests_console/lobby
	name = "Announcement Console"

/obj/machinery/requests_console/lobby/Initialize()
	. = ..()
	SSticker.lobby.announcers += src

/obj/machinery/requests_console/lobby/Destroy()
	SSticker.lobby.announcers -= src
	return ..()

/obj/machinery/requests_console/lobby/attack_hand()
	return