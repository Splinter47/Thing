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
	
	$clusterID = $_POST["clusterID"];
	$timeStamp = $_POST["time"];
	
	$query = "	SELECT * 
				FROM  `Things` 
				";
				//WHERE  `cluster_id` = $clusterID
	
	function GetBlocks($thing) {
	
	
		//query here to find the most liked 
		$thingID = $thing->ID;
		
		$innerQuery = "	SELECT `image`, MAX(likes) AS HighestBlock 
				FROM  `blocks`
				WHERE  `thing_id` = $thingID 
				";
				
		global $db;
				
		$blocks = $db->select($innerQuery);
		
		if(count($blocks)!=0){
			$topBlockImage = $blocks[0]->image;
		}
	
		$keyValuePairs = array(
		
			"id" 			=> 	$thingID,
			"title"			=> 	$thing->title,
			"post_time"		=>	$thing->post_time,
			"image"			=>	$topBlockImage
		);
		
		return $keyValuePairs;
	}
	
	$result = $encoder->Encode($query, "GetBlocks");
	
	echo $result."<!time!>".$timeStamp;
	
?>