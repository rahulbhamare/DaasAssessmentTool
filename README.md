# DaasAssessmentTool

DaasAssessmentTool also called Readiness tool, assess system environment for installing Daas, TPM , TA client, AirWatch etc. 
It is used to, 
	      
	       - Verify network connectivity using HTTP standard request/response. Refer example in section **Configuration File**       
	       - Connect end points using Auto detect, Static and Manual proxy settings. Refer example in section **Input JSON File**
	       - Generate TM client log zip at C:/ path	  Refer example in section **Command test case has following fields :**     
	       - Support for Authenticated proxy.  Refer example in section **Input JSON File** 	       	       
	       - Generate InstallShield logs at C:/ path  	       
	       - Check Running Status of Service on local machine  
	       - Check Enrolled Status of Process on local machine  
	       - Added support to read DAAS Agent registry keys
Note: For more examples, please refer to the InputJsonFile Folder provided in AppBundle
              
## Configuration File

Purpose : config_file.json is to create different test suites for different environment like Daas, TPM, TA client, AirWatch etc.
config_file.json contains path to InputJsonFiles, that has json files for different environment as described in example below.  
Similarly, one can add support for any new environment in future. 

It contains following fields: 

	name - name of the test suite 
	purpose - purpose to test 
	path - path of the json file from which the test case data will be taken at runtime 
	type - type of test case. It has two different types 
	
	   a. http - check network connectivity 
		
	      e.g. { 
	      		"name": "DAAS", 
			"purpose": "Connecting DAAS end points :", 
			"path": "InputJsonFiles\https_tests_daas.json", 
			"type": "http" 
		   } 
		   
	   b. command - execute commands through Cmd 
		
	      e.g. {
	      		"name": "TM client Log", 
			"purpose": "Generating TM client Log", 
			"path": "InputJsonFiles\command_tests_cases.json", 
			"type": "command" 
		   } 
		       
        #####**Please note: Field engineer can add/edit test suit in config_file.json with existing format.**
 
## Input JSON File 

 Purpose : Input json files contains **array of http & command test case** require to assess a particular environment.
           **HTTP test case has following common fields :**
	    
	    name - name of the test case
            type - type of test case
            target - url to check network connectivity
            expected_response_code - field engineer should provide expected response code here.Once HttpWebRequest
	                             has been sent then this expected response code should match with Actual response
				     code. Otherwise, the result will be false.
           
		For https_tests_proxy.json input file having some additional fields like, 
	        	a. proxyserver - proxy server url, to be used to provide static proxy.
                	b. username, password - to check authenticated proxies.
			c. Tool can also detect manual IE proxy settings as well as Auto proxy/LAN settings.
	
              	e.g. "testcases": [
	                {
	                 "name": "HP Daas",
	                 "type": "http",
	                 "target": "http://jenkins-server.lightaria.com:8080/login",      
	                 "port": 8080,
	                 "proxyserver":"web-proxy.in.hpicorp.net:8080",
	                 "username": "uname",
	                 "password": "pwd",
	                 "expected_response_code": 200,
	                 "notes": "Optional field."
	                }
	              ]
	Note:  Optional Fields:  proxyserver, username, password, notes.
	
   **Command test case has following fields :**
	  
	  name - name of test case
	  purpose - purpose of test case. It should be one of following,
	  	    	a. ServiceStatus : Command test is about to check Service running status on local machine
			b. ProcessStatus : Command test is about to check Process enrollement status on local machine
			c. RegistryStatus: Indicates, command test is about to verify whether mentioned Registry path present or not on local machine
			d. InstallExe    : Indicates, command test case is about to install provided exe file				
	  type - "command",
	  command - It can be Service name, Process name, Registry path or, exe path depends on purpose field (Refer json files for more examples)
	  expected_response_code - Expected code which command should return. '1' indicates True, Otherwise False.
	  params - additional information require for command test case. It is optional field
	  
	  Command test case e.g. 
	  {
      		"name": "Installshield Installer logs",
      		"purpose": "InstallExe",
      		"type": "command",
      		"command": "C:\\Setup.exe",
      		"expected_response_code": 1,
      		"params" : "\/debuglog\"C:\\setupexe.log\""
    	  }			                    
## How to Add/Edit url to check network connectivity?

 Field engineer can edit config_file.json to add test suite. Also, he can edit/add input json files to add test case.

## Prerequisites to run DaasAssessmentTool 

  DaasAssessmentTool is developed into two different templates as follows :
   - DaasAssessmentToolConsole 
   	To run this tool:  DaasAssessmentToolConsole.exe -t "config_file.json".
   - DaasAssessmentToolUI
      
   The field engineer should require setup .exe of either one of them.
 
 ## Hardware Requirements
 
 A standalone tool runs on Windows 7 SP1, windows 8.1 and windows 10 OS without external dependencies. 
