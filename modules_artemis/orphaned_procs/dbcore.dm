DBQuery/Execute(sql_query=src.sql,cursor_handler=default_cursor)
	Close()
	. = _dm_db_execute(_db_query,sql_query,db_connection._db_con,cursor_handler,null)
	if(!.)	//No connection try to reconnect
		dbcon.Connect()
		. = _dm_db_execute(_db_query,sql_query,db_connection._db_con,cursor_handler,null)
	return