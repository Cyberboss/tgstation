//A special mob with permission to live before the round starts
/mob/living/carbon/human/lobby
    name = "Glitch in the Matrix"
	status_flags = GODMODE

    var/prefers_observer = FALSE
    var/spawning = FALSE

    var/obj/screen/splash/splash_screen
    var/datum/callback/roundstart_callback
    var/datum/action/lobby/setup_character/setup_character
    var/datum/action/lobby/become_observer/become_observer
    var/datum/action/lobby/show_player_polls/show_player_polls

INITIALIZE_IMMEDIATE(/mob/living/carbon/human/lobby)

/mob/living/carbon/human/lobby/Initialize()
    . = ..()
    GLOB.lobby_players += src
    forceMove(pick(GLOB.newplayer_start))
	equipOutfit(/datum/outfit/vr_basic, FALSE)

    setup_character = new
    setup_character.Grant(src)
    become_observer = new
    become_observer.Grant(src)
    show_player_polls = new
    show_player_polls.Grant(src)

    verbs += /mob/dead/proc/server_hop

    roundstart_callback = CALLBACK(src, ./proc/OnRoundstart)
    
	notransform = SSticker.current_state <= GAME_STATE_PREGAME

/mob/living/carbon/human/lobby/Destroy()
    QDEL_NULL(setup_character)
    QDEL_NULL(become_observer)
    QDEL_NULL(show_player_polls)
    LAZYREMOVE(SSticker.round_start_events, roundstart_callback)
    QDEL_NULL(roundstart_callback)
    GLOB.lobby_players -= src
    return ..()

/mob/living/carbon/human/lobby/proc/OnInitializationsComplete()
    notransform = FALSE
	window_flash(client, ignorepref = TRUE) //let them know lobby has opened up.
    if(prefers_observer && make_me_an_observer())
        return

/mob/living/carbon/human/lobby/proc/OnRoundstart()
    notransform = TRUE

/mob/living/carbon/human/lobby/proc/PhaseOut()
    do_sparks(5, FALSE, src)
    qdel(src)

/mob/living/carbon/human/lobby/proc/make_me_an_observer()
	var/this_is_like_playing_right = alert(src,"Are you sure you wish to observe? You will not be able to play this round!","Player Setup","Yes","No")

	if(QDELETED(src) || !src.client || this_is_like_playing_right != "Yes")
		return FALSE

	var/mob/dead/observer/observer = new
	spawning = TRUE
	observer.started_as_observer = TRUE
	var/obj/effect/landmark/observer_start/O = locate(/obj/effect/landmark/observer_start) in GLOB.landmarks_list
	to_chat(src, "<span class='notice'>Now teleporting.</span>")
	if (O)
		observer.forceMove(O.loc)
	else
		to_chat(src, "<span class='notice'>Teleporting failed. Ahelp an admin please</span>")
		stack_trace("There's no freaking observer landmark available on this map!")
        qdel(observer)
        return

	observer.key = key
	observer.set_ghost_appearance()
	if(observer.client && observer.client.prefs)
		observer.real_name = observer.client.prefs.real_name
		observer.name = observer.real_name
	observer.update_icon()
	observer.stop_sound_channel(CHANNEL_LOBBYMUSIC)
	QDEL_NULL(mind)
    PhaseOut()
	return TRUE

/mob/living/carbon/human/lobby/Life()
    return

/mob/living/carbon/human/lobby/Stat()
    ..()
    LobbyStat()

/mob/living/carbon/human/lobby/forceMove(atom/destination)
    loc = destination

/mob/living/carbon/human/lobby/Login()
    ..()
    name = client.key
    splash_screen = new(client)
    SSticker.OnRoundstart(roundstart_callback)

/mob/living/carbon/human/lobby/Logout()
	..()
	if(!spawning)//Here so that if they are spawning and log out, the other procs can play out and they will have a mob to come back to.
		key = null//We null their key before deleting the mob, so they are properly kicked out.
		qdel(src)

/mob/living/carbon/human/lobby/say(message)
    client.ooc(message)