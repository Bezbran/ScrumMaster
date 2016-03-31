# ScrumMaster
App which allows any scrum team to manage them working flow.

To begin USING the ScrumMaster application, you need to 
  1. Download the files from "ScrumMaster\ScrumMasterServGUI\bin\Debug".
  2. Run the ScrumMasterServGUI.exe with your parameters. This app needs to run all the time to recive users actions.
  3. Download the files from "ScrumMaster\ScrumMasterClient\bin\Debug".
  3. Run the ScrumMasterClient.exe and login as the manager user.
  4. Add some users, you must add scrum-master user and product-owner user.
  5. Every user need to run ScrumMasterClient.exe from his computer and login.
  6. Thats all.
Now the scrum-master user can create sprint and after that the product-owner can create userstorys,
the all users can add tasks and take ownerships on them.

To begin CONTRIBUTE you need to
  1. Clone the repo.
  2. Create a new branch with the name of what feature you want add.
  3. Write the code.
  4. Asks us to merge it to master branch in order that everyone can thank you.
  
The ScrumMaster contains 3 projects:
  1. (ScrumMasterWcf) The WCF dll project which contains the necessary classes and etc for the ScrumMaster.
  2. (ScrumMasterServGUI) The EXE file which wraps the WCF DLL file and allows the user to run it with a tiny GUI.
    This EXE can be run in test mode by adding "test" as a command line argumment.
  3. (ScrumMasterClient) The borring project :). The EXE file that allows each user from the team to connect to
    ScrumMaster server and adding\deleting\changing tasks or take ownership on tasks.
    In addition, it allows the manager to adds user, and allows the scrum-master user to declare new sprint, 
    and allows the product-owner user to create new userstorys.

In ScrumMasterServGUI test-mode the app will create simple scrum-team which contains
  Team name: TEST_Run
  Manager name: ManagerUser
  10 team members
  Passwords: 1234
  1 sprint. start time: now, duration time: 14 days
  10 userstorys where each one contains 10 tasks
  Run at (IP:PORT/PATH): http:/defaultip:10002/ScrumMaster. defaultip can be found in the ScrumMasterServGUI screen.

Only scrum-master user can create sprint and only the product-owner can create userstorys.
Only task can be in Impediment status and only userstory can be in Accept status.
Only manager can add / remove / change other user. Each user can update himself details.
To edit details of task / userstory / user one can just double-click on them details.
