<?php
    function debug_print($array) {
        echo "<pre>";
        print_r($array);
        echo "</pre>";
    }

    /*
        Bekommt eine Variable uebergeben, die einen Ordner darstellt:
        Anschließend wird geprueft, ob sich Dateien im Ordner befinden
        Falls Ordner leer ist => true
        ansonsten => false

    */

    function is_dir_empty($dir) {
        $handle = opendir($dir);
        while(false !== ($entry = readdir($handle))) {
            if ($entry !="." && $entry != "..") {
                closedir($handle);
                return FALSE;
            }
        }
        cloesdir($handle);
        return TRUE;
     }

     /*
        Bekommt Variable (Sollte wieder Ordner sein): 
        Falls es sich um einen Ordner handelt => wird jede Datei, die in diesem zu finden ist geloescht
        falls Dateien geloescht worden sind => Rueckgabe true
        sonst false


     */

     function delete_files_in_dir ($dirname) {
         if (is_dir($dirname)) {
             $dir_handle = opendir($dirname);
         }
         if(!$dir_handle) {
             return false;
         }
         while($file = readdir($dir_handle)) {
             if($file != "." && $file != "..") {
                 if(!is_dir($dirname."/".$file)) {
                     unlink($dirname."/".$file);
                 } else {
                     delete_files_in_dir($dirname."/".$file);
                 }
             }
         }
         closedir($dir_handle);
         return true;
     }

?>

<html>
    <title>Validierungstool</title>
    <link href="//maxcdn.bootstrapcdn.com/font-awesome/4.1.0/css/font-awesome.min.css" rel="stylesheet">
    <link href='https://fonts.googleapis.com/css?family=Lato' rel='stylesheet' type='text/css'>
    <link href='https://fonts.googleapis.com/css?family=Montserrat' rel='stylesheet' 
    type='text/css'>

    <div class = "header">
        <h1> Validierungstool für XRechnung 
            <style>
               
            </style>
        
        </h1>
    </div>
<body>
    <form action="" method = "POST" enctype = "multipart/form-data">
        <input type="file" name="userfile">
        
    <input type="submit" value="Upload" />
    </form>
    
    <form method = "POST">
    <p>
        <button name="valid">Validieren</button>
        
        <style>
        .btn {
            position: absolute;
            text-align: center;
            cursor: pointer;
            background: blue;
            color: white;
        }
        body {
            text-align: center;
            
        }
         h1 {
                    background-color: #34495e;
                    color: white;
        }
        </style>
        
    </p>
    </form>

    

    <?php

         if(isset($_FILES["userfile"])) {
            $php_upload_errors = array (
                0 => "No errors found, the upload was successful",
                1 => "The uploaded file exceeds the upload_max_filesize directive in php.ini",
                2 => "The uploaded file exceeds the MAX_FILE_SIZE directive specified in the HTML form",
                3 => "The uploaded file was only partially uploaded",
                4 => "No file was uploaded",
                6 => "Missing a temporary folder",
                7 => "Failed to write file to disk",
                8 => "A PHP extension stopped the file upload",);
            $userfile_name = $_FILES["userfile"]["name"];
    
            $ext_err = false;
            $extensions = array("xml");
            $ext_file = explode(".",$_FILES["userfile"]["name"]);
            $ext_file = end($ext_file);
            
    
            if(!in_array($ext_file,$extensions)) {
                $ext_err=true;
            }
            // error != 0 => errors
            if($_FILES["userfile"]["error"]) {
                echo $php_upload_errors[$_FILES["userfile"]["error"]];
            } elseif($ext_err) {
                echo "Invalid file extensions for ".$userfile_name;
            } else {
                echo "The upload of ".$userfile_name." was successful";
            }
            $tmp="invoices_tmp/";
            if(!empty($tmp)) {
                delete_files_in_dir($tmp);
            }
        
            if(in_array($ext_file,$extensions)) {
            move_uploaded_file($_FILES["userfile"]["tmp_name"],$tmp.
                                $_FILES["userfile"]["name"]);
            }
        }

        if(isset($_POST["valid"])) {
            $dir = "invoices_tmp";
            $files_dir = array_slice(scandir($dir),2);
            if(empty($files_dir)) {
                echo "You have to upload a file";
            } else {
                $path_and_file = "invoices_tmp/";
                exec("java -jar validationtool-standaloneRelease.jar -s scenarios.xml -h ".$path_and_file  ,$output);
                foreach($output as $out) {
                    echo $out;
                }
                echo "The file has been validated";
                // integrated show function
                $file1 = $files_dir[0];
                $starting_file = explode(".",$file1);
                $starting_file = $starting_file[0];
                $search_for_file =$starting_file."-report.html";
                if(file_exists($search_for_file)) {
                    $report_file_name = $starting_file."-report.";
                    readfile($report_file_name."html");
                    unlink($report_file_name."html");
                    unlink($report_file_name."xml");
                }
                //deletes every file from the tmp directory
                delete_files_in_dir($path_and_file);
            }
            
        }




        /*
        For earlier SHOW button

         if(isset($_POST["show"])) {
            $dir = "invoices_tmp";
            $files_dir = array_slice(scandir($dir),2);
            if(empty($files_dir)) {
                echo "At first you have to upload and validate a file";
            } else {
            $file1 = $files_dir[0];
            $starting_file = explode(".",$file1);
            $starting_file = $starting_file[0];
            $search_for_file =$starting_file."-report.html";
            if(file_exists($search_for_file)) {
            readfile($starting_file."-report.html");
            } else {
                echo "Cannot show unvalidated files";
            }
            }
        }
        */
    ?>

</body>
</html>