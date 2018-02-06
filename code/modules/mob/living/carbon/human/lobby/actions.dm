/datum/action/lobby
	icon_icon = 'icons/mob/actions/actions_lobby.dmi'

/datum/action/lobby/ApplyIcon(obj/screen/movable/action_button/current_button, force = FALSE)
	. = ..()
	//so the buttons are always up to date before initializations
	COMPILE_OVERLAYS(current_button)

/datum/action/lobby/setup_character
	name = "Setup Character"
	desc = "Create your character and change game preferences"
	button_icon_state = "setup_character"

/datum/action/lobby/setup_character/Trigger()
	. = ..()
	if(.)
		owner.client.prefs.ShowChoices()

/datum/action/lobby/become_observer
	name = "Observe"
	desc = "Become a ghost and watch the round"
	button_icon_state = "observe"
	var/on = FALSE

/datum/action/lobby/become_observer/Trigger()
	. = ..()
	if(!.)
		return
	on = !on
	UpdateButtonIcon()
	if(on)
		var/mob/living/carbon/human/lobby/player = owner
		player.make_me_an_observer()

/datum/action/lobby/become_observer/UpdateButtonIcon()
	background_icon_state = on ? "template_active" : initial(background_icon_state)
	return ..()

/datum/action/lobby/show_player_polls
	name = "Show Player Polls"
	desc = "Show active playerbase polls. Not available to guests"
	button_icon_state = "show_polls"
	var/newpoll

/datum/action/lobby/show_player_polls/IsAvailable()
	if(!owner || IsGuestKey(owner.key) || !SSdbcore.Connect())
		return FALSE
	return ..()

/datum/action/lobby/show_player_polls/proc/CheckDB()
	var/isadmin = owner.client && owner.client.holder
	var/datum/DBQuery/query_get_new_polls = SSdbcore.NewQuery("SELECT id FROM [format_table_name("poll_question")] WHERE [(isadmin ? "" : "adminonly = false AND")] Now() BETWEEN starttime AND endtime AND id NOT IN (SELECT pollid FROM [format_table_name("poll_vote")] WHERE ckey = \"[owner.ckey]\") AND id NOT IN (SELECT pollid FROM [format_table_name("poll_textreply")] WHERE ckey = \"[owner.ckey]\")")
	if(query_get_new_polls.Execute() && query_get_new_polls.NextRow())
		button_icon_state = "show_polls_new"
		UpdateButtonIcon()

/datum/action/lobby/show_player_polls/Trigger()
	. = ..()
	if(!.)
		return
	var/mob/living/carbon/human/lobby/player = owner
	player.handle_player_polling()
