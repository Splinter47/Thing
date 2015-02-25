<?php
	require_once('class-db.php');
	
	if ( !class_exists('INSERT') ) {
		class INSERT {
			public function update_user($user_id, $postdata) {
				global $db;
				
				$table = 's_users';
				
				$query = "
								UPDATE $table
								SET user_email='$postdata[user_email]', user_pass='$postdata[user_pass]', user_nicename='$postdata[user_nicename]'
								WHERE ID=$user_id
							";

				return $db->update($query);
			}
			
			public function add_friend($user_id, $friend_id) {
				global $db;
				
				$table = 's_friends';
				
				$query = "
								INSERT INTO $table (user_id, friend_id)
								VALUES ('$user_id', '$friend_id')
							";
				
				return $db->insert($query);
			}
			
			public function remove_friend($user_id, $friend_id) {
				global $db;
				
				$table = 's_friends';
				
				$query = "
								DELETE FROM $table 
								WHERE user_id = '$user_id'
								AND friend_id = '$friend_id'
							";
				
				return $db->insert($query);
			}
			
			public function add_status($user_id, $post) {
				global $db;
				
				$table = 's_status';
				
				$query = "
								INSERT INTO $table (user_id, status_time, status_content)
								VALUES ($user_id, '$post[status_time]', '$post[status_content]')
							";
				
				return $db->insert($query);
			}
			
			public function send_message($post) {
				global $db;
				
				$table = 's_messages';
				
				$query = "
								INSERT INTO $table (message_time, message_sender_id, message_recipient_id, message_subject, message_content)
								VALUES ('$post[message_time]', '$post[message_sender_id]', '$post[message_recipient_id]', '$post[message_subject]', '$post[message_content]')
							";
				
				return $db->insert($query);
			}
		}
	}
	
	$insert = new INSERT;
?>