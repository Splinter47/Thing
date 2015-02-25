<?php
	require_once('includes/class-query.php');
	
	$filters = $_POST["filters"];
	$timeStamp = $_POST["time"];
	
	//make a wordpress query
    $args = array( 	'post_type' => 'staff',
    				'posts_per_page' => 40,
    				'tax_query'		=> $query->WPTaxQueryAND($filters, 'project_region')
    				);
    				
    $WPQuery = new WP_Query( $args );
	
	
	//create a callback function for inside the loop
	function GetStaff(){
		
		//find all the terms
		$terms = wp_get_post_terms(get_the_ID(), "project_region");
		$termString = "";
		foreach($terms as $term){
			$termString .= $term->slug.",";
		}
	
		$keyValuePairs = array(
			
			"id" 			=> 	get_the_ID(),
			"salutation"	=> 	get_field( 'salutation' ),
			"firstname"		=>	get_field( 'firstname' ),
			"surname"		=>	get_field( 'surname' ),
			"job_title"		=>	get_field( 'job_title' ),
			"qualAcademic"	=>	get_field( 'qualifications_academic' ),
			"qualProf"		=>	get_field( 'qualifications_professional' ),
			"jobShort"		=>	get_field( 'job_description_short' ),
			"jobLong"		=>	get_field( 'job_description_long' ),
			"value"			=>	get_field( 'value'),
			"expShort"		=>	get_field( 'experience_short' ),
			"expLong"		=>	get_field( 'experience_long' ),
			"office"		=>	get_field( 'office' ),
			"image"			=>	wp_get_attachment_url(get_post_thumbnail_id(get_the_ID()), 'full'),
			"terms"			=>	$termString
		);
		
		return $keyValuePairs;
	}
	
	$result = $query->WPEncode($WPQuery, "GetStaff");
	
	echo $result."<!time!>".$timeStamp;
?>