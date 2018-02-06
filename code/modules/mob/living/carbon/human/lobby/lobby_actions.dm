/datum/action/lobby
    icon_icon = 'icons/mob/actions/lobby_actions.dmi'

/datum/action/lobby/ApplyIcon()
    . = ..()
    COMPILE_OVERLAYS(current_button)

/datum/action/lobby/setup_character
    name = "Setup Character"
    desc = "Create your character and change game preferences"

/datum/action/lobby/setup_character/Trigger()
    . = ..()
    if(.)
        owner.client.prefs.ShowChoices()

/datum/action/lobby/become_observer
    name = "Observe"
    desc = "Become a ghost"
    var/in_input = FALSE

/datum/action/lobby/become_observer/IsAvailable()
    return !in_input && ..()

/datum/action/lobby/become_observer/Trigger()
    . = ..()
    if(!.)
        return
    
    in_input = TRUE
    //TODO

/datum/action/lobby/show_player_polls
    name = "Show Player Polls"
    desc = "Show active playerbase polls. Not available to guests"
    button_icon_state = "show_polls"
    var/newpoll

/datum/action/lobby/show_player_polls/IsAvailable()
    if(!owner || IsGuestKey(owner.key) || !SSdbcore.Connect())
        return FALSE
    
    . = ..()

    if(newpoll != null)
        return

	var/isadmin = owner.client && owner.client.holder
    var/datum/DBQuery/query_get_new_polls = SSdbcore.NewQuery("SELECT id FROM [format_table_name("poll_question")] WHERE [(isadmin ? "" : "adminonly = false AND")] Now() BETWEEN starttime AND endtime AND id NOT IN (SELECT pollid FROM [format_table_name("poll_vote")] WHERE ckey = \"[owner.ckey]\") AND id NOT IN (SELECT pollid FROM [format_table_name("poll_textreply")] WHERE ckey = \"[owner.ckey]\")")
    if(query_get_new_polls.Execute() && query_get_new_polls.NextRow())
        button_icon_state = "show_polls_new"

/datum/action/lobby/show_player_polls/Trigger()
    . = ..()
    if(.)
        var/mob/living/carbon/human/lobby/player = owner
        player.handle_polling()