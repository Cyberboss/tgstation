/obj/machinery/door/poddoor/preopen/lobby
	name = "Crew Boarding Room"

/obj/machinery/door/poddoor/preopen/lobby/Initialize()
	. = ..()
	SSticker.lobby.shutters += src

/obj/machinery/door/poddoor/preopen/lobby/Destroy()
	SSticker.lobby.shutters -= src
	return ..()

/obj/machinery/teleport/hub/lobby
	icon_state = "tele1"

/obj/machinery/teleport/hub/lobby/link_power_station()
	return

/obj/machinery/teleport/hub/lobby/CollidedWith(mob/living/carbon/human/lobby/player)
	if(istype(player))
		player.AttemptJoin()
		return

	//garbage
	do_sparks(5, FALSE, src)
	qdel(player)

/turf/open/floor/light/lobby
	name = "Crew Boarding Room"
	coloredlights = list("b")

/turf/open/floor/light/lobby/Initialize()
	. = ..()
	SSticker.lobby.lights += src

/turf/open/floor/light/lobby/Destroy()
	SSticker.lobby.lights -= src
	return ..()

/turf/open/floor/light/lobby/proc/WarningSequence()
	coloredlights = list("r", "g")

/turf/open/floor/light/lobby/proc/Normalize()
	coloredlights = initial(coloredlights)

/obj/machinery/computer/lobby/setup_character
	name = "Setup Character"

/obj/machinery/computer/lobby/setup_character/attack_hand(mob/player)
	player.client.prefs.ShowChoices()

/obj/machinery/computer/lobby/observer
	icon_screen = "comm_monitor"
	name = "Become Observer"

/obj/machinery/computer/lobby/observer/attack_hand(mob/living/carbon/human/lobby/player)
	player.make_me_an_observer()

/obj/machinery/computer/lobby/poll
	icon_screen = "syndishuttle"
	name = "Show Player Polls"

/obj/machinery/computer/lobby/poll/attack_hand(mob/living/carbon/human/lobby/player)
	player.handle_player_polling()
