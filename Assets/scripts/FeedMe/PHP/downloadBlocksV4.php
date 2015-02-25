<?php
	require_once('AdvancedLogin/load.php');
	require_once('includes/wp-sql-encoder.php');
	
	$userCookie = $_POST["userCookie"];
	$passCookie = $_POST["passCookie"];
	
	/*$logged = $j->checkLogin($userCookie, $passCookie);
	
	if($logged != true){
		echo "failed";
		die();
	}*/
	
	$thingID = $_POST["thingID"];
	$timeStamp = $_POST["time"];
	
	$query = "	SELECT * 
				FROM  `blocks` 
				WHERE  `thing_id` = $thingID
				";
	
	function GetBlocks($thing) {
	
		$keyValuePairs = array(
		
			"id" 			=> 	$thing->id,
			"title"			=> 	$thing->title,
			"author_id"		=>	$thing->author_id,
			"image"			=>	$thing->image,
			"thing_id"		=>	$thing->thing_id
		);
		
		return $keyValuePairs;
	}
	
	$result = $encoder->Encode($query, "GetBlocks");
	
	echo $result."<!time!>".$timeStamp;
	
?>