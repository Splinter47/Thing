<?php
	require_once('class-db.php');
	
	//require files for wordpress
	//require_once(__DIR__ ."/"."../../wordpress/wp-load.php");

	if ( !class_exists('QUERY') ) {
		class QUERY {
		
			public function Encode($query, $encodeFunction) {
				global $db;
				
				$things = $db->select($query);
				
				$bigString = "";
				foreach ( $things as $thing ) {
				
					$keyValuePairs = call_user_func($encodeFunction, $thing);
				
					foreach($keyValuePairs as $key => $value){
            				$bigString .= $this->EncodeField($key, $value);
            		}
					
					// final delimiter is end of block marker
					$bigString .= "<!!>";
				}
				
				return $bigString;
			}
			
			public function WPEncode($WPQuery, $encodeFunction){
				$bigString = "";
			
				//if posts exist print them all out
    			if ($WPQuery->have_posts()){
        			while ( $WPQuery->have_posts() ){

            			//get the next post
            			$WPQuery->the_post();
            			$keyValuePairs = call_user_func($encodeFunction);
            			
            			foreach($keyValuePairs as $key => $value){
            				$bigString .= $this->EncodeField($key, $value);
            			}
					
						// final delimiter is end of block marker
						$bigString .= "<!!>";
        			}
					wp_reset_postdata();
    			}
				
				return $bigString;
			}
			
			public function EncodeField($key, $value){
					$del = array("<!>", "<!!>", "<!!!>");
					$valueNoDelim = str_replace($del, "!!", html_entity_decode($value, ENT_NOQUOTES, 'UTF-8'));
					$output = $key.$del[2].strip_tags($valueNoDelim).$del[0];
					return $output;
			}
			
			public function WPTaxQueryOR($termsString, $taxonomy){
				return $this->WPTaxQuery($termsString, $taxonomy, "OR");
			}
			
			public function WPTaxQueryAND($termsString, $taxonomy){
				return $this->WPTaxQuery($termsString, $taxonomy, "AND");
			}
			
			public function WPTaxQuery($termsString, $taxonomy, $relation){
			
				if($termsString == ""){
					return "";
				}else{
					//divide the list into taxonomies to filter
					$terms = explode(",", $termsString);
				
					$taxQuery = array('relation' => $relation);
				
					foreach($terms as $term){
				
						$singleTax = array(	'taxonomy' => $taxonomy,
											'field'    => 'slug',
											'terms'    => array( $term )
											);
				
						array_push($taxQuery, $singleTax);
					}
				
					return $taxQuery;
				}
			}
		}
	}
	
	$encoder = new QUERY;
?>