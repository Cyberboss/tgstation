//No breathing/reagents/whatever please. Those subsystems aren't running
/mob/living/carbon/human/lobby/Life()
	return

//Needful
/mob/living/carbon/human/lobby/Stat()
	..()
	LobbyStat()

//OOC only
/mob/living/carbon/human/lobby/say(message)
	if(client.ooc(message))
		ShowSpeechBubble(message)

//do it right
/mob/living/carbon/human/lobby/suicide()
	if(new_character || notransform)
		return
	make_me_an_observer()

//prevent hearing ambience and stuff during transitions
/mob/living/carbon/human/lobby/can_hear()
	return !notransform

//no griff
/mob/living/carbon/human/lobby/can_be_pulled()
	return FALSE

//nahh, don't worry about us
/mob/living/carbon/human/lobby/update_z()
	return
