/datum/action/lobby
	icon_icon = 'icons/mob/actions/actions_lobby.dmi'

/datum/action/lobby/ApplyIcon(obj/screen/movable/action_button/current_button, force = FALSE)
	. = ..()
	//so the buttons are always up to date before initializations
	COMPILE_OVERLAYS(current_button)

	current_button.layer = ABOVE_SPLASHSCREEN_LAYER
	current_button.plane = ABOVE_SPLASHSCREEN_PLANE

/datum/action/lobby/setup_character
	name = "Setup Character"
	desc = "Create your character and change game preferences"
	button_icon_state = "setup_character"

/datum/action/lobby/setup_character/Trigger()
	. = ..()
	if(.)
		owner.client.prefs.ShowChoices(owner)

/datum/action/lobby/show_player_polls
	name = "Show Player Polls"
	desc = "Show active playerbase polls. Not available to guests"
	button_icon_state = "show_polls"

/datum/action/lobby/show_player_polls/Trigger()
	. = ..()
	if(!.)
		return
	var/mob/living/carbon/human/lobby/player = owner
	player.handle_player_polling()
