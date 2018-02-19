# DaasAssessmentTool

This is Readiness tool to assess system environment for installing Daas, TPM , TA client etc.
It is used to, 

	       - Verify network connectivity using C# native Http classes
	       - Generate TM client log zip at C:/ path

## Configuration file

Purpose : config_file.json is to create different test suits for different environment like Daas, TPM, TA client etc. 
It contains following fields, 

	name - name of the test suit 
	purpose - purpose to test 
	path - path of the json file from which the test case data will be taken at runtime 
	type - type of test case. It has two different types 
	
	   a. http - check network connectivity 
		
	      e.g. { "name": "DAAS", "purpose": "Connecting DAAS end points :", "path": "InputJsonFiles\https_tests_daas.json",
		     "type": "http" } 
	   b. command - execute commands through Cmd 
		
	      e.g. { "name": "TM client Log", "purpose": "Generating TM client Log", "path": "InputJsonFiles\command_tests_cases.json",
                       "type": "command" } 
		       
		Please note: Field engineer can add/edit test suit in config_file.json with existing format.
 
## Input JSON file 

 Purpose : Input json files contains array of test cases require to assess a particular environment.
           It has following common fields,
	   
              name - name of the test case
              type - type of test case
              target - url to check network connectivity
              expected_response_code - field engineer should provide expected response code here.Once HttpWebRequest has been sent then                                        this expected response code should match with Actual response code. Otherwise, the result will be                                        false.
              
	      notes - it is optional for additional paramaeters like request headers etc.
 
           For https_tests_proxy.json input file having some additional fields like,
              proxyserver - proxy server url
              username, password - to check authenticated proxies
	
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

## How to Add/Edit url to check network connectivity?

 Field engineer can edit config_file.json to add test suite. Also, he can edit/add input json files to add test case.

## Prerequisites to run DaasAssessmentTool 

  DaasAssessmentTool is developed into two different templates as follows :
   - DaasAssessmentToolConsole 
   - DaasAssessmentToolUI 
   The field engineer should require setup .exe of eighter one of them.

