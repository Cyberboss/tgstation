
/var/const/access_energy_barrier = 69 //Spacepod energy barriers
/var/const/access_desubber = 70 //Supermatter Desublimation lab
/var/const/access_iaa = 71 //Internal Affairs
/var/const/access_fax = 72 //Fax machines
/var/const/access_foreman = 73 //Mining foreman offices

get_all_accesses()
	return list(access_security, access_sec_doors, access_brig, access_armory, access_forensics_lockers, access_court,
	            access_medical, access_genetics, access_morgue, access_rd,
	            access_tox, access_tox_storage, access_chemistry, access_engine, access_engine_equip, access_maint_tunnels,
	            access_external_airlocks, access_change_ids, access_ai_upload,
	            access_teleporter, access_eva, access_heads, access_captain, access_all_personal_lockers,
	            access_tech_storage, access_chapel_office, access_atmospherics, access_kitchen,
	            access_bar, access_janitor, access_crematorium, access_robotics, access_cargo, access_construction,
	            access_hydroponics, access_library, access_lawyer, access_virology, access_cmo, access_qm, access_surgery,
	            access_theatre, access_research, access_mining, access_mailsorting, access_weapons,
	            access_heads_vault, access_mining_station, access_xenobiology, access_ce, access_hop, access_hos, access_RC_announce,
	            access_keycard_auth, access_tcomsat, access_gateway, access_mineral_storeroom, access_minisat, access_network, access_cloning,
	            access_energy_barrier, access_desubber, access_iaa, access_fax, access_foreman)

get_region_accesses(code)
	switch(code)
		if(0)
			return get_all_accesses()
		if(1) //station general
			return list(access_kitchen,access_bar, access_hydroponics, access_janitor, access_chapel_office, access_crematorium, access_library, access_theatre, access_lawyer)
		if(2) //security
			return list(access_sec_doors, access_weapons, access_security, access_brig, access_armory, access_forensics_lockers, access_court, access_hos)
		if(3) //medbay
			return list(access_medical, access_genetics, access_cloning, access_morgue, access_chemistry, access_virology, access_surgery, access_cmo)
		if(4) //research
			return list(access_research, access_tox, access_tox_storage, access_genetics, access_robotics, access_xenobiology, access_minisat, access_rd, access_network)
		if(5) //engineering and maintenance
			return list(access_construction, access_maint_tunnels, access_engine, access_engine_equip, access_external_airlocks, access_tech_storage, access_atmospherics, access_tcomsat, access_minisat, access_ce, access_desubber)
		if(6) //supply
			return list(access_mailsorting, access_mining, access_mining_station, access_mineral_storeroom, access_cargo, access_qm, access_foreman)
		if(7) //command
			return list(access_heads, access_RC_announce, access_keycard_auth, access_change_ids, access_ai_upload, access_teleporter, access_eva, access_gateway, access_all_personal_lockers, access_heads_vault, access_hop, access_captain, access_iaa, access_energy_barrier)

get_region_accesses_name(code)
	switch(code)
		if(0)
			return "All"
		if(1) //station general
			return "General"
		if(2) //security
			return "Security"
		if(3) //medbay
			return "Medbay"
		if(4) //research
			return "Research"
		if(5) //engineering and maintenance
			return "Engineering"
		if(6) //supply
			return "Supply"
		if(7) //command
			return "Command"

get_access_desc(A)
	switch(A)
		if(access_cargo)
			return "Cargo Bay"
		if(access_cargo_bot)
			return "Delivery Chutes"
		if(access_security)
			return "Security"
		if(access_brig)
			return "Holding Cells"
		if(access_court)
			return "Courtroom"
		if(access_forensics_lockers)
			return "Forensics"
		if(access_medical)
			return "Medical"
		if(access_genetics)
			return "Genetics Lab"
		if(access_morgue)
			return "Morgue"
		if(access_tox)
			return "R&D Lab"
		if(access_tox_storage)
			return "Toxins Lab"
		if(access_chemistry)
			return "Chemistry Lab"
		if(access_rd)
			return "RD Office"
		if(access_bar)
			return "Bar"
		if(access_janitor)
			return "Custodial Closet"
		if(access_engine)
			return "Engineering"
		if(access_engine_equip)
			return "Power Equipment"
		if(access_maint_tunnels)
			return "Maintenance"
		if(access_external_airlocks)
			return "External Airlocks"
		if(access_emergency_storage)
			return "Emergency Storage"
		if(access_change_ids)
			return "ID Console"
		if(access_ai_upload)
			return "AI Chambers"
		if(access_teleporter)
			return "Teleporter"
		if(access_eva)
			return "EVA"
		if(access_heads)
			return "Bridge"
		if(access_captain)
			return "Captain"
		if(access_all_personal_lockers)
			return "Personal Lockers"
		if(access_chapel_office)
			return "Chapel Office"
		if(access_tech_storage)
			return "Technical Storage"
		if(access_atmospherics)
			return "Atmospherics"
		if(access_crematorium)
			return "Crematorium"
		if(access_armory)
			return "Armory"
		if(access_construction)
			return "Construction"
		if(access_kitchen)
			return "Kitchen"
		if(access_hydroponics)
			return "Hydroponics"
		if(access_library)
			return "Library"
		if(access_lawyer)
			return "Law Office"
		if(access_robotics)
			return "Robotics"
		if(access_virology)
			return "Virology"
		if(access_cmo)
			return "CMO Office"
		if(access_qm)
			return "Quartermaster"
		if(access_surgery)
			return "Surgery"
		if(access_theatre)
			return "Theatre"
		if(access_manufacturing)
			return "Manufacturing"
		if(access_research)
			return "Science"
		if(access_mining)
			return "Mining"
		if(access_mining_office)
			return "Mining Office"
		if(access_mailsorting)
			return "Cargo Office"
		if(access_mint)
			return "Mint"
		if(access_mint_vault)
			return "Mint Vault"
		if(access_heads_vault)
			return "Main Vault"
		if(access_mining_station)
			return "Mining EVA"
		if(access_xenobiology)
			return "Xenobiology Lab"
		if(access_hop)
			return "HoP Office"
		if(access_hos)
			return "HoS Office"
		if(access_ce)
			return "CE Office"
		if(access_RC_announce)
			return "RC Announcements"
		if(access_keycard_auth)
			return "Keycode Auth."
		if(access_tcomsat)
			return "Telecommunications"
		if(access_gateway)
			return "Gateway"
		if(access_sec_doors)
			return "Brig"
		if(access_mineral_storeroom)
			return "Mineral Storage"
		if(access_minisat)
			return "AI Satellite"
		if(access_weapons)
			return "Weapon Permit"
		if(access_network)
			return "Network Access"
		if(access_cloning)
			return "Cloning Room"
		if(access_energy_barrier)
			return "Energy Barriers"
		if(access_iaa)
			return "Internal Affairs"
		if(access_desubber)
			return "Desublimation Lab"
		if(access_fax)
			return "Fax Machines"
		if(access_foreman)
			return "Mining Foreman"

get_all_jobs()
	return list("Assistant", "Captain", "Head of Personnel", "Bartender", "Cook", "Botanist", "Quartermaster", "Cargo Technician",
				"Shaft Miner", "Janitor", "Librarian", "Chaplain", "Chief Engineer", "Station Engineer","Senior Engineer",
				"Chief Medical Officer", "Medical Doctor", "Senior Medical Doctor", "Internal Affairs Agent", "SolGov Representative",
				"Research Director", "Scientist", "Senior Scientist", "Head of Security", "Warden", "Detective", "Security Officer")