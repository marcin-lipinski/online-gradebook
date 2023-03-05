# online-gradebook
Project written in a two-person team. The language used to communicate with the user is Polish.

  The solution simulates the operations of an online gradebook. It features two projects: Client and Server. The processes communicate with each other through a named pipe. The process Client is the user interface. Instructions entered by the user are sent to the Server process. This one, executes them through operations on the database and returns the result of these actions. The client passes them on to the user.
  
  In order to build a properly working project, a "relational database" was built. Its tables are stored in the form of JSON files. The example database is created by using a script.
  
   The system allows users to log in to one of four account types: student, parent, teacher and admin. The student account allows you to view the subjects and grades to which the it is assigned to. The parent account allows the same action - displaying the child's grades. As a simplification, one parent has one child in the system. The teacher's account allows view students and their grades within the specific class and specific subject this teacher manages. He can also add and delete grades for them. The administrator account allows adding students and removing students from classes. In addition, the client process displays the user's login time.
