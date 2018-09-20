#load "StaticUtils.csx"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text.RegularExpressions;


    public static class StaticUtils
    {
        //Conversation data key
        public static string CurrentIntent = "CurrentIntent";
        public static string QueryName = "QueryName";
        public static string AnswerName = "AnswerName";
        public static string EntityList = "EntityList";
        public static string ParamNumberAnswer = "ParamNumberAnswer";
        /// <summary>
        /// User data key
        /// </summary>
        public static string UserIdentity = "UserIdentity";
        /// <summary>
        /// Activity type for module info
        /// </summary>
        public static string RestrictModuleID = "RestrictModuleID";

        /// <summary>
        ///Module DICTIONARY contains Module Name and ID, USED TO avoid another call to luis which is expensive
        /// </summary>
        public static Dictionary<string, IReadOnlyList<string>> Module = new Dictionary<string, IReadOnlyList<string>>()
             {
                {"COM121",new List<string> {"Mathematics I" , "COM121 Mathematics I", "Mathematics I COM121", "COM 121", "Mathematics I COM 121", "COM 121 Mathematics I", "Mathematics 1", "Mathematics 1 COM 121", "COM 121 Mathematics 1", "Mathematics 1 COM121", "COM121 Mathematics 1"} },
                {"COM136",new List<string> {"Programming I" , "COM136 Programming I", "Programming I COM136", "COM 136", "Programming I COM 136", "COM 136 Programming I", "Programming 1", "Programming 1 COM 136", "COM 136 Programming 1", "Programming 1 COM136", "COM136 Programming 1"} },
                {"COM176",new List<string> {"Introduction to Computer Games" , "COM176 Introduction to Computer Games", "Introduction to Computer Games COM176", "COM 176", "Introduction to Computer Games COM 176", "COM 176 Introduction to Computer Games", "Introduction to Computer Game", "Introduction to Computer Game COM 176", "COM 176 Introduction to Computer Game", "Introduction to Computer Game COM176", "COM176 Introduction to Computer Game"} },
                {"COM178",new List<string> {"Systems Analysis & Design" , "COM178 Systems Analysis & Design", "Systems Analysis & Design COM178", "COM 178", "Systems Analysis & Design COM 178", "COM 178 Systems Analysis & Design", "Systems Analysis and Design", "Systems Analysis and Design COM 178", "COM 178 Systems Analysis and Design", "Systems Analysis and Design COM178", "COM178 Systems Analysis and Design"} },
                {"EEE107",new List<string> {"Mathematics for Engineering" , "EEE107 Mathematics for Engineering", "Mathematics for Engineering EEE107", "EEE 107", "Mathematics for Engineering EEE 107", "EEE 107 Mathematics for Engineering", "Mathematic for Engineering", "Mathematic for Engineering EEE 107", "EEE 107 Mathematic for Engineering", "Mathematic for Engineering EEE107", "EEE107 Mathematic for Engineering"} },
                {"EEE185",new List<string> {"Digital Electronics" , "EEE185 Digital Electronics", "Digital Electronics EEE185", "EEE 185", "Digital Electronics EEE 185", "EEE 185 Digital Electronics", "Digital Electronic", "Digital Electronic EEE 185", "EEE 185 Digital Electronic", "Digital Electronic EEE185", "EEE185 Digital Electronic"} },
                {"MEC106",new List<string> {"Design and CAD" , "MEC106 Design and CAD", "Design and CAD MEC106", "MEC 106", "Design and CAD MEC 106", "MEC 106 Design and CAD", "Design & CAD", "Design & CAD MEC 106", "MEC 106 Design & CAD", "Design & CAD MEC106", "MEC106 Design & CAD"} },
                {"MEC109",new List<string> {"Manufacturing Processes" , "MEC109  Manufacturing Processes", "Manufacturing Processes MEC109", "MEC 09", "Manufacturing Processes MEC 09", "MEC 09  Manufacturing Processes", "Manufacturing Process", "Manufacturing Process MEC 09", "MEC 09  Manufacturing Process", "Manufacturing Process MEC109", "MEC109  Manufacturing Process"} },
                {"COM321",new List<string> {"Visual Programming" , "COM321 Visual Programming", "Visual Programming COM321", "COM 321", "Visual Programming COM 321", "COM 321 Visual Programming"} },
                {"COM326",new List<string> {"Object-oriented Programming" , "COM326 Object-oriented Programming", "Object-oriented Programming COM326", "COM 326", "Object-oriented Programming COM 326", "COM 326 Object-oriented Programming", "Object Oriented Programming", "Object Oriented Programming COM 326", "COM 326 Object Oriented Programming", "Object Oriented Programming COM326", "COM326 Object Oriented Programming"} },
                {"COM414",new List<string> {"Internet & Multimedia Authoring" , "COM414 Internet & Multimedia Authoring", "Internet & Multimedia Authoring COM414", "COM 414", "Internet & Multimedia Authoring COM 414", "COM 414 Internet & Multimedia Authoring", "Internet and Multimedia Authoring", "Internet and Multimedia Authoring COM 414", "COM 414 Internet and Multimedia Authoring", "Internet and Multimedia Authoring COM414", "COM414 Internet and Multimedia Authoring"} },
                {"COM417",new List<string> {"Professional Issues" , "COM417 Professional Issues", "Professional Issues COM417", "COM 417", "Professional Issues COM 417", "COM 417 Professional Issues", "Professional Issue", "Professional Issue COM 417", "COM 417 Professional Issue", "Professional Issue COM417", "COM417 Professional Issue"} },
                {"COM419",new List<string> {"Object-Oriented Modelling" , "COM419 Object-Oriented Modelling", "Object-Oriented Modelling COM419", "COM 419", "Object-Oriented Modelling COM 419", "COM 419 Object-Oriented Modelling", "Object Oriented Modelling", "Object Oriented Modelling COM 419", "COM 419 Object Oriented Modelling", "Object Oriented Modelling COM419", "COM419 Object Oriented Modelling"} },
                {"COM420",new List<string> {"Mathematics II" , "COM420 Mathematics II", "Mathematics II COM420", "COM 420", "Mathematics II COM 420", "COM 420 Mathematics II", "Mathematics 2", "Mathematics 2 COM 420", "COM 420 Mathematics 2", "Mathematics 2 COM420", "COM420 Mathematics 2"} },
                {"EEE366",new List<string> {"Engineering Analysis" , "EEE366  Engineering Analysis", "Engineering Analysis EEE366", "EEE 366", "Engineering Analysis EEE 366", "EEE 366  Engineering Analysis"} },
                {"MEC308",new List<string> {"Materials" , "MEC308 Materials", "Materials MEC308", "MEC 308", "Materials MEC 308", "MEC 308 Materials", "Material", "Material MEC 308", "MEC 308 Material", "Material MEC308", "MEC308 Material"} },
                {"MEC318",new List<string> {"Fluid Dynamics & Thermodynamics 2A" , "MEC318 Fluid Dynamics & Thermodynamics 2A", "Fluid Dynamics & Thermodynamics 2A MEC318", "MEC 318", "Fluid Dynamics & Thermodynamics 2A MEC 318", "MEC 318 Fluid Dynamics & Thermodynamics 2A", "Fluid Dynamics and Thermodynamics 2A", "Fluid Dynamics and Thermodynamics 2A MEC 318", "MEC 318 Fluid Dynamics and Thermodynamics 2A", "Fluid Dynamics and Thermodynamics 2A MEC318", "MEC318 Fluid Dynamics and Thermodynamics 2A"} },
                {"MEC337",new List<string> {"Statics Dynamics 2" , "MEC337 Statics Dynamics 2", "Statics Dynamics 2 MEC337", "MEC 337", "Statics Dynamics 2 MEC 337", "MEC 337 Statics Dynamics 2"} },
                {"MEC360",new List<string> {"Statics and Dynamics II" , "MEC360 Statics and Dynamics II", "Statics and Dynamics II MEC360", "MEC 360", "Statics and Dynamics II MEC 360", "MEC 360 Statics and Dynamics II", "Statics & Dynamics II", "Statics & Dynamics II MEC 360", "MEC 360 Statics & Dynamics II", "Statics & Dynamics II MEC360", "MEC360 Statics & Dynamics II"} },
                {"ENE302",new List<string> {"Electrical and Energy Engineering" , "ENE302 Electrical and Energy Engineering", "Electrical and Energy Engineering ENE302", "ENE 302", "Electrical and Energy Engineering ENE 302", "ENE 302 Electrical and Energy Engineering"} },
                {"COM529",new List<string> {"Research Studies & Project Management" , "COM529 Research Studies & Project Management", "Research Studies & Project Management COM529", "COM 529", "Research Studies & Project Management COM 529", "COM 529 Research Studies & Project Management", "Research Studies and Project Management", "Research Studies and Project Management COM 529", "COM 529 Research Studies and Project Management", "Research Studies and Project Management COM529", "COM529 Research Studies and Project Management"} },
                {"COM535",new List<string> {"Systems� Security" , "COM535 Systems� Security", "Systems� Security COM535", "COM 535", "Systems� Security COM 535", "COM 535 Systems� Security", "System Security", "System Security COM 535", "COM 535 System Security", "System Security COM535", "COM535 System Security"} },
                {"COM540",new List<string> {"Knowledge Management" , "COM540 Knowledge Management", "Knowledge Management COM540", "COM 540", "Knowledge Management COM 540", "COM 540 Knowledge Management"} },
                {"COM580",new List<string> {"Enterprise Computing" , "COM580 Enterprise Computing", "Enterprise Computing COM580", "COM 580", "Enterprise Computing COM 580", "COM 580 Enterprise Computing"} },
                {"COM594",new List<string> {"Mobile Technology" , "COM594 Mobile Technology", "Mobile Technology COM594", "COM 594", "Mobile Technology COM 594", "COM 594 Mobile Technology"} },
                {"COM631",new List<string> {"Interactive Multimedia Games Development" , "COM631 Interactive Multimedia Games Development", "Interactive Multimedia Games Development COM631", "COM 631", "Interactive Multimedia Games Development COM 631", "COM 631 Interactive Multimedia Games Development", "Interactive Multimedia Game Development", "Interactive Multimedia Game Development COM 631", "COM 631 Interactive Multimedia Game Development", "Interactive Multimedia Game Development COM631", "COM631 Interactive Multimedia Game Development"} },
                {"EEE540",new List<string> {"Programmable Logic Systems" , "EEE540 Programmable Logic Systems", "Programmable Logic Systems EEE540", "EEE 540", "Programmable Logic Systems EEE 540", "EEE 540 Programmable Logic Systems", "Programmable Logic System", "Programmable Logic System EEE 540", "EEE 540 Programmable Logic System", "Programmable Logic System EEE540", "EEE540 Programmable Logic System"} },
                {"EEE542",new List<string> {"Mixed Signal Design" , "EEE542 Mixed Signal Design", "Mixed Signal Design EEE542", "EEE 542", "Mixed Signal Design EEE 542", "EEE 542 Mixed Signal Design"} },
                {"EEE521",new List<string> {"Final Year Project" , "EEE521 Final Year Project", "Final Year Project EEE521", "EEE 521", "Final Year Project EEE 521", "EEE 521 Final Year Project"} },
                {"MEC505",new List<string> {"Design & Industrial Apps" , "MEC505 Design & Industrial Apps", "Design & Industrial Apps MEC505", "MEC 505", "Design & Industrial Apps MEC 505", "MEC 505 Design & Industrial Apps", "Design and Industrial Apps", "Design and Industrial Apps MEC 505", "MEC 505 Design and Industrial Apps", "Design and Industrial Apps MEC505", "MEC505 Design and Industrial Apps"} },
                {"MEC510",new List<string> {"Mechanical Science 3" , "MEC510 Mechanical Science 3", "Mechanical Science 3 MEC510", "MEC 510", "Mechanical Science 3 MEC 510", "MEC 510 Mechanical Science 3", "Mechanical Science III", "Mechanical Science III MEC 510", "MEC 510 Mechanical Science III", "Mechanical Science III MEC510", "MEC510 Mechanical Science III"} },
                {"COM558",new List<string> {"Professional Software Development I" , "COM558 Professional Software Development I", "Professional Software Development I COM558", "COM 558", "Professional Software Development I COM 558", "COM 558 Professional Software Development I", "Professional Software Development 1", "Professional Software Development 1 COM 558", "COM 558 Professional Software Development 1", "Professional Software Development 1 COM558", "COM558 Professional Software Development 1"} },
                {"COM809",new List<string> {"Professional Software Development II" , "COM809 Professional Software Development II", "Professional Software Development II COM809", "COM 809", "Professional Software Development II COM 809", "COM 809 Professional Software Development II", "Professional Software Development 2", "Professional Software Development 2 COM 809", "COM 809 Professional Software Development 2", "Professional Software Development 2 COM809", "COM809 Professional Software Development 2"} },
                {"COM557",new List<string> {"Computer Hardware" , "COM557 Computer Hardware", "Computer Hardware COM557", "COM 557", "Computer Hardware COM 557", "COM 557 Computer Hardware"} },
                {"COM808",new List<string> {"Operating System Fundamentals" , "COM808 Operating System Fundamentals", "Operating System Fundamentals COM808", "COM 808", "Operating System Fundamentals COM 808", "COM 808 Operating System Fundamentals", "Operating System Fundamental", "Operating System Fundamental COM 808", "COM 808 Operating System Fundamental", "Operating System Fundamental COM808", "COM808 Operating System Fundamental"} },
                {"COM139",new List<string> {"Programming II" , "COM139 Programming II", "Programming II COM139", "COM 139", "Programming II COM 139", "COM 139 Programming II", "Programming 2", "Programming 2 COM 139", "COM 139 Programming 2", "Programming 2 COM139", "COM139 Programming 2"} },
                {"COM162",new List<string> {"Database Systems" , "COM162 Database Systems", "Database Systems COM162", "COM 162", "Database Systems COM 162", "COM 162 Database Systems", "Database System", "Database System COM 162", "COM 162 Database System", "Database System COM162", "COM162 Database System"} },
                {"COM167",new List<string> {"Computer Hardware and organization" , "COM167 Computer Hardware and organization", "Computer Hardware and organization COM167", "COM 167", "Computer Hardware and organization COM 167", "COM 167 Computer Hardware and organization", "Computer Hardware & organization", "Computer Hardware & organization COM 167", "COM 167 Computer Hardware & organization", "Computer Hardware & organization COM167", "COM167 Computer Hardware & organization"} },
                {"COM192",new List<string> {"Comp Information Systems Technologies" , "COM192 Comp Information Systems Technologies", "Comp Information Systems Technologies COM192", "COM 192", "Comp Information Systems Technologies COM 192", "COM 192 Comp Information Systems Technologies"} },
                {"COM193",new List<string> {"Games Design" , "COM193 Games Design", "Games Design COM193", "COM 193", "Games Design COM 193", "COM 193 Games Design", "Game Design", "Game Design COM 193", "COM 193 Game Design", "Game Design COM193", "COM193 Game Design"} },
                {"COM320",new List<string> {"Computer Network & op Systems" , "COM320 Computer Network & op Systems", "Computer Network & op Systems COM320", "COM 320", "Computer Network & op Systems COM 320", "COM 320 Computer Network & op Systems", "Computer Network and op Systems", "Computer Network and op Systems COM 320", "COM 320 Computer Network and op Systems", "Computer Network and op Systems COM320", "COM320 Computer Network and op Systems"} },
                {"COM328",new List<string> {"Algorithms and Data Structures" , "COM328 Algorithms and Data Structures", "Algorithms and Data Structures COM328", "COM 328", "Algorithms and Data Structures COM 328", "COM 328 Algorithms and Data Structures", "Algorithm and Data Structure", "Algorithm and Data Structure COM 328", "COM 328 Algorithm and Data Structure", "Algorithm and Data Structure COM328", "COM328 Algorithm and Data Structure"} },
                {"COM350",new List<string> {"Interactive Interface Design" , "COM350 Interactive Interface Design", "Interactive Interface Design COM350", "COM 350", "Interactive Interface Design COM 350", "COM 350 Interactive Interface Design"} },
                {"COM428",new List<string> {"Games Graphics Programming" , "COM428 Games Graphics Programming", "Games Graphics Programming COM428", "COM 428", "Games Graphics Programming COM 428", "COM 428 Games Graphics Programming", "Game Graphics Programming", "Game Graphics Programming COM 428", "COM 428 Game Graphics Programming", "Game Graphics Programming COM428", "COM428 Game Graphics Programming"} },
                {"COM429",new List<string> {"Multimedia Games Development" , "COM429 Multimedia Games Development", "Multimedia Games Development COM429", "COM 429", "Multimedia Games Development COM 429", "COM 429 Multimedia Games Development", "Multimedia Game Development", "Multimedia Game Development COM 429", "COM 429 Multimedia Game Development", "Multimedia Game Development COM429", "COM429 Multimedia Game Development"} },
                {"COM531",new List<string> {"Multimedia Technologies" , "COM531 Multimedia Technologies", "Multimedia Technologies COM531", "COM 531", "Multimedia Technologies COM 531", "COM 531 Multimedia Technologies", "Multimedia Technologie", "Multimedia Technologie COM 531", "COM 531 Multimedia Technologie", "Multimedia Technologie COM531", "COM531 Multimedia Technologie"} },
                {"COM561",new List<string> {"Concurrent Distributed Systems" , "COM561 Concurrent Distributed Systems", "Concurrent Distributed Systems COM561", "COM 561", "Concurrent Distributed Systems COM 561", "COM 561 Concurrent Distributed Systems", "Concurrent Distributed System", "Concurrent Distributed System COM 561", "COM 561 Concurrent Distributed System", "Concurrent Distributed System COM561", "COM561 Concurrent Distributed System"} },
                {"COM565",new List<string> {"Software Engineering" , "COM565 Software Engineering", "Software Engineering COM565", "COM 565", "Software Engineering COM 565", "COM 565 Software Engineering"} },
                {"COM581",new List<string> {"Networking Operating Systems" , "COM581 Networking Operating Systems", "Networking Operating Systems COM581", "COM 581", "Networking Operating Systems COM 581", "COM 581 Networking Operating Systems", "Networking Operating System", "Networking Operating System COM 581", "COM 581 Networking Operating System", "Networking Operating System COM581", "COM581 Networking Operating System"} },
                {"COM596",new List<string> {"Mobile Robotics" , "COM596 Mobile Robotics", "Mobile Robotics COM596", "COM 596", "Mobile Robotics COM 596", "COM 596 Mobile Robotics", "Mobile Robotic", "Mobile Robotic COM 596", "COM 596 Mobile Robotic", "Mobile Robotic COM596", "COM596 Mobile Robotic"} },
                {"COM621",new List<string> {"Interactive Web Development" , "COM621 Interactive Web Development", "Interactive Web Development COM621", "COM 621", "Interactive Web Development COM 621", "COM 621 Interactive Web Development"} },
                {"COM623",new List<string> {"Intelligence Systems" , "COM623 Intelligence Systems", "Intelligence Systems COM623", "COM 623", "Intelligence Systems COM 623", "COM 623 Intelligence Systems", "Intelligence System", "Intelligence System COM 623", "COM 623 Intelligence System", "Intelligence System COM623", "COM623 Intelligence System"} },
                {"COM629",new List<string> {"Mobile Game Development" , "COM629 Mobile Game Development", "Mobile Game Development COM629", "COM 629", "Mobile Game Development COM 629", "COM 629 Mobile Game Development", "Mobile Games Development", "Mobile Games Development COM 629", "COM 629 Mobile Games Development", "Mobile Games Development COM629", "COM629 Mobile Games Development"} },
                {"COM810",new List<string> {"Database Systems" , "COM810 Database Systems", "Database Systems COM810", "COM 810", "Database Systems COM 810", "COM 810 Database Systems", "Database System", "Database System COM 810", "COM 810 Database System", "Database System COM810", "COM810 Database System"} },
                {"COM811",new List<string> {"Mobile Devices and Applications" , "COM811 Mobile Devices and Applications", "Mobile Devices and Applications COM811", "COM 811", "Mobile Devices and Applications COM 811", "COM 811 Mobile Devices and Applications", "Mobile Devices and Application", "Mobile Devices and Application COM 811", "COM 811 Mobile Devices and Application", "Mobile Devices and Application COM811", "COM811 Mobile Devices and Application"} },
                {"COM812",new List<string> {"Data Structures" , "COM812 Data Structures", "Data Structures COM812", "COM 812", "Data Structures COM 812", "COM 812 Data Structures", "Data Structure", "Data Structure COM 812", "COM 812 Data Structure", "Data Structure COM812", "COM812 Data Structure"} },
                {"COM813",new List<string> {"Concurrent Systems" , "COM813 Concurrent Systems", "Concurrent Systems COM813", "COM 813", "Concurrent Systems COM 813", "COM 813 Concurrent Systems", "Concurrent System", "Concurrent System COM 813", "COM 813 Concurrent System", "Concurrent System COM813", "COM813 Concurrent System"} },
                {"EEE186",new List<string> {"Analogue Electronics" , "EEE186 Analogue Electronics", "Analogue Electronics EEE186", "EEE 186", "Analogue Electronics EEE 186", "EEE 186 Analogue Electronics", "Analogue Electronic", "Analogue Electronic EEE 186", "EEE 186 Analogue Electronic", "Analogue Electronic EEE186", "EEE186 Analogue Electronic"} },
                {"EEE201",new List<string> {"Electronic and Electromagnetic Circuits" , "EEE201 Electronic and Electromagnetic Circuits", "Electronic and Electromagnetic Circuits EEE201", "EEE 201", "Electronic and Electromagnetic Circuits EEE 201", "EEE 201 Electronic and Electromagnetic Circuits", "Electronics and Electromagnetics Circuits", "Electronics and Electromagnetics Circuits EEE 201", "EEE 201 Electronics and Electromagnetics Circuits", "Electronics and Electromagnetics Circuits EEE201", "EEE201 Electronics and Electromagnetics Circuits"} },
                {"EEE421",new List<string> {"Digital Systems Design" , "EEE421 Digital Systems Design", "Digital Systems Design EEE421", "EEE 421", "Digital Systems Design EEE 421", "EEE 421 Digital Systems Design", "Digital System Design", "Digital System Design EEE 421", "EEE 421 Digital System Design", "Digital System Design EEE421", "EEE421 Digital System Design"} },
                {"EEE422",new List<string> {"Electronics Systems Design" , "EEE422 Electronics Systems Design", "Electronics Systems Design EEE422", "EEE 422", "Electronics Systems Design EEE 422", "EEE 422 Electronics Systems Design", "Electronic System Design", "Electronic System Design EEE 422", "EEE 422 Electronic System Design", "Electronic System Design EEE422", "EEE422 Electronic System Design"} },
                {"EEE428",new List<string> {"Electrical Engineering Services" , "EEE428 Electrical Engineering Services", "Electrical Engineering Services EEE428", "EEE 428", "Electrical Engineering Services EEE 428", "EEE 428 Electrical Engineering Services", "Electronic Engineering Services", "Electronic Engineering Services EEE 428", "EEE 428 Electronic Engineering Services", "Electronic Engineering Services EEE428", "EEE428 Electronic Engineering Services"} },
                {"MEC102",new List<string> {"Introduction to Statics & Dynamics" , "MEC102 Introduction to Statics & Dynamics", "Introduction to Statics & Dynamics MEC102", "MEC 102", "Introduction to Statics & Dynamics MEC 102", "MEC 102 Introduction to Statics & Dynamics", "Introduction to Statics and Dynamics", "Introduction to Statics and Dynamics MEC 102", "MEC 102 Introduction to Statics and Dynamics", "Introduction to Statics and Dynamics MEC102", "MEC102 Introduction to Statics and Dynamics"} },
                {"MEC124",new List<string> {"Introduction to Fluid Mechanics" , "MEC124 Introduction to Fluid Mechanics", "Introduction to Fluid Mechanics MEC124", "MEC 124", "Introduction to Fluid Mechanics MEC 124", "MEC 124 Introduction to Fluid Mechanics", "Introduction to Fluid Mechanic", "Introduction to Fluid Mechanic MEC 124", "MEC 124 Introduction to Fluid Mechanic", "Introduction to Fluid Mechanic MEC124", "MEC124 Introduction to Fluid Mechanic"} },
                {"MEC304",new List<string> {"Design & Industrial Applications" , "MEC304 Design & Industrial Applications", "Design & Industrial Applications MEC304", "MEC 304", "Design & Industrial Applications MEC 304", "MEC 304 Design & Industrial Applications", "Design and Industrial Applications", "Design and Industrial Applications MEC 304", "MEC 304 Design and Industrial Applications", "Design and Industrial Applications MEC304", "MEC304 Design and Industrial Applications"} },
                {"MEC354",new List<string> {"Industrial Management & Law" , "MEC354 Industrial Management & Law", "Industrial Management & Law MEC354", "MEC 354", "Industrial Management & Law MEC 354", "MEC 354 Industrial Management & Law", "Industrial Management and Law", "Industrial Management and Law MEC 354", "MEC 354 Industrial Management and Law", "Industrial Management and Law MEC354", "MEC354 Industrial Management and Law"} },
                {"MEC502",new List<string> {"Computer Aided Engineering" , "MEC502 Computer Aided Engineering", "Computer Aided Engineering MEC502", "MEC 502", "Computer Aided Engineering MEC 502", "MEC 502 Computer Aided Engineering"} },
                {"MEC517",new List<string> {"Manufacturing Technology" , "MEC517 Manufacturing Technology", "Manufacturing Technology MEC517", "MEC 517", "Manufacturing Technology MEC 517", "MEC 517 Manufacturing Technology"} }
        };
        public static Dictionary<string, IReadOnlyList<string>> AssessmentNumber = new Dictionary<string, IReadOnlyList<string>>()
             {
                {"Assignment 1",new List<string> {"1","one","first assignment", "first coursework", "coursework 1","coursework1", "assignment1","assignment 1","First Assignment","First Coursework",  "first Assignment", "first Coursework", "First assignment","First coursework","Coursework 1","Coursework1", "Assignment1","Assignment 1"} },
                {"Assignment 2",new List<string> {"2","two","second assignment", "second coursework","coursework 2",   "coursework2",  "assignment2",  "assignment 2", "Second Assignment",  "Second Coursework",  "second Assignment",   "second Coursework",    "Second assignment",    "Second coursework",      "Coursework 2",  "Coursework2", "Assignment2","Assignment 2"} },
                {"Assignment 3",new List<string> {"third assignment",  "third coursework",  "coursework 3",  "coursework3", "assignment3", "assignment 3", "Third Assignment",   "Third Coursework",  "third Assignment",  "third Coursework",  "Third assignment", "Third coursework",  "Coursework 3",  "Coursework3", "Assignment3", "Assignment 3"} },
             };
        public static Dictionary<string, IReadOnlyList<string>> Week = new Dictionary<string, IReadOnlyList<string>>()
             {
                {"1",new List<string> {"week 1","Week1", "week1", "Week 1" } },
                {"2",new List<string> {"week 2", "Week2", "week2", "Week 2" } },
                {"3",new List<string> {"week 3", "Week3", "week3", "Week 3" } },
                {"4",new List<string> {"week 4", "Week4", "week4", "Week 4" } },
                {"5",new List<string> {"week 5", "Week5", "week5", "Week 5" } },
                {"6",new List<string> {"week 6", "Week6", "week6", "Week 6" } },
                {"7",new List<string> {"week 7", "Week7", "week7", "Week 7" } },
                {"8",new List<string> {"week 8", "Week8", "week8", "Week 8" } },
                {"9",new List<string> {"week 9", "Week9", "week9", "Week 9" } },
                {"10",new List<string> {"week 10", "Week10", "week10", "Week 10" } },
                {"11",new List<string> {"week 11", "Week11", "week11", "Week 11" } },
                {"12",new List<string> {"week 12", "Week12", "week12", "Week 12" } },
                {"13",new List<string> {"week 13", "Week13", "week13", "Week 13" } },
                {"14",new List<string> {"week 14", "Week14", "week14", "Week 14" } },
                {"15",new List<string> {"week 15", "Week15", "week15", "Week 15" } },
                {"16",new List<string> {"week 16", "Week16","week16", "Week 16" } },


             };
    

            public static Dictionary<string, IReadOnlyList <string>> LectureTypeDic = new Dictionary<string, IReadOnlyList<string>>()
             {
                {"Lab",new List<string>{ "lab,laboratory,Laboratory" } },
                {"Lecture",new List<string>{"lecture"}} 
             };
        public static Dictionary<string, string> WeekNumber = new Dictionary<string, string>()
             {
                
                {"week 1","1"},
                {"week 2","2"},
                {"week 3","3" },
                {"week 4","4"} ,
                {"week 5","5"},
                {"week 6","6"},
                {"week 7","7"} ,
                {"week 8","8"} ,
                {"week 9","9" },
                {"week 10","10" },
                {"week 11","11" },
                {"week 12","12" },
                {"week 13","13" },
                {"week 14","14" },
                {"week 15","15" },
                {"week 16","16" },
                {"Week 1","1"},
                {"Week 2","2"},
                {"Week 3","3" },
                {"Week 4","4"} ,
                {"Week 5","5"},
                {"Week 6","6"},
                {"Week 7","7"} ,
                {"Week 8","8"} ,
                {"Week 9","9" },
                {"Week 10","10" },
                {"Week 11","11" },
                {"Week 12","12" },
                {"Week 13","13" },
                {"Week 14","14" },
                {"Week 15","15" },
                {"Week 16","16" }


             };
        /// <summary>
        ///LECTURER DICTIONARY contains Lecturer name, USED TO avoid another call to luis which is expensive
        /// </summary>
        public static Dictionary<string, IReadOnlyList<string>> Lecturer = new Dictionary<string, IReadOnlyList<string>>()
        {
            {"Dr Hamed Bahmani",new List<string> {"Hamed Bahmani" , "Dr Bahmani", "Dr Hamed Bahmani' s", "Hamed Bahmani' s", "Dr Bahmani' s"} },
            {"Mr Michael Callaghan",new List<string> {"Michael Callaghan" , "Mr Callaghan", "Mr Michael Callaghan' s", "Michael Callaghan' s", "Mr Callaghan' s"} },
            {"Dr Hubert Cecotti",new List<string> {"Hubert Cecotti" , "Dr Cecotti", "Dr Hubert Cecotti' s", "Hubert Cecotti' s", "Dr Cecotti' s"} },
            {"Professor Sonya Coleman",new List<string> {"Sonya Coleman" , "Professor Coleman", "Professor Sonya Coleman' s", "Sonya Coleman' s", "Professor Coleman' s"} },
            {"Dr Joan Condell",new List<string> {"Joan Condell" , "Dr Condell", "Dr Joan Condell' s", "Joan Condell' s", "Dr Condell' s"} },
            {"Professor Damien Coyle",new List<string> {"Damien Coyle" , "Professor Coyle", "Professor Damien Coyle' s", "Damien Coyle' s", "Professor Coyle' s"} },
            {"Professor Kevin Curran",new List<string> {"Kevin Curran" , "Professor  Curran", "Professor Kevin Curran' s", "Kevin Curran' s", "Professor  Curran' s"} },
            {"Dr Xuemei Ding",new List<string> {"Xuemei Ding" , "Dr Ding", "Dr Xuemei Ding' s", "Xuemei Ding' s", "Dr Ding' s"} },
            {"Dr Bryan Gardiner",new List<string> {"Bryan Gardiner" , "Dr Gardiner", "Dr Bryan Gardiner' s", "Bryan Gardiner' s", "Dr Gardiner' s"} },
            {"Dr Abas Hadawey",new List<string> {"Abas Hadawey" , "Dr Hadawey", "Dr Abas Hadawey' s", "Abas Hadawey' s", "Dr Hadawey' s"} },
            {"Dr Jim Harkin",new List<string> {"Jim Harkin" , "Dr Harkin", "Dr Jim Harkin' s", "Jim Harkin' s", "Dr Harkin' s"} },
            {"Dr Daniel Kelly",new List<string> {"Daniel Kelly" , "Dr Kelly", "Dr Daniel Kelly' s", "Daniel Kelly' s", "Dr Kelly' s"} },
            {"Dr Dermot Kerr",new List<string> {"Dermot Kerr" , "Dr Kerr", "Dr Dermot Kerr' s", "Dermot Kerr' s", "Dr Kerr' s"} },
            {"Mr  Emmett Kerr",new List<string> {"Emmett Kerr" , "Mr Kerr", "Mr  Emmett Kerr ' s", "Emmett Kerr ' s", "Mr Kerr ' s"} },
            {"Dr Tom Lunney",new List<string> {"Tom Lunney" , "Dr Lunney", "Dr Tom Lunney' s", "Tom Lunney' s", "Dr Lunney' s"} },
            {"Mr Aiden McCaughey",new List<string> {"Aiden McCaughey" , "Mr McCaughey", "Mr Aiden McCaughey' s", "Aiden McCaughey' s", "Mr McCaughey' s"} },
            {"Dr Karl McCreadie",new List<string> {"Karl McCreadie" , "Dr McCreadie", "Dr Karl McCreadie' s", "Karl McCreadie' s", "Dr McCreadie' s"} },
            {"Professor Liam McDaid",new List<string> {"Liam McDaid" , "Professor McDaid", "Professor Liam McDaid' s", "Liam McDaid' s", "Professor McDaid' s"} },
            {"Mr Malachy McElholm",new List<string> {"Malachy McElholm" , "Mr McElholm", "Mr Malachy McElholm' s", "Malachy McElholm' s", "Mr McElholm' s"} },
            {"Dr Shaun McFadden",new List<string> {"Shaun McFadden" , "Dr McFadden", "Dr Shaun McFadden' s", "Shaun McFadden' s", "Dr McFadden' s"} },
            {"Dr Sandra Moffett",new List<string> {"Sandra Moffett" , "Dr Moffett", "Dr Sandra Moffett' s", "Sandra Moffett' s", "Dr Moffett' s"} },
            {"Mrs Mairin Nicell",new List<string> {"Mairin Nicell" , "Mrs Nicell", "Mrs Mairin Nicell' s", "Mairin Nicell' s", "Mrs Nicell' s"} },
            {"Professor Girijesh Prasad",new List<string> {"Girijesh Prasad" , "Professor Prasad", "Professor Girijesh Prasad' s", "Girijesh Prasad' s", "Professor Prasad' s"} },
            {"Dr Justin Quinn",new List<string> {"Justin Quinn" , "Dr Quinn", "Dr Justin Quinn' s", "Justin Quinn' s", "Dr Quinn' s"} },
            {"Dr Inaki Rano",new List<string> {"Inaki Rano" , "Dr Rano", "Dr Inaki Rano' s", "Inaki Rano' s", "Dr Rano' s"} },
            {"Dr Nazmul Siddique",new List<string> {"Nazmul Siddique" , "Dr Siddique", "Dr Nazmul Siddique' s", "Nazmul Siddique' s", "Dr Siddique' s"} },
            {"Dr Philip Vance",new List<string> {"Philip Vance" , "Dr Vance", "Dr Philip Vance' s", "Philip Vance' s", "Dr Vance' s"} },
            {"Dr Shane Wilson",new List<string> {"Shane Wilson" , "Dr Wilson", "Dr Shane Wilson' s", "Shane Wilson' s", "Dr Wilson' s"} },
            {"Dr Kongfatt  Wong-Lin",new List<string> {"Kongfatt  Wong-Lin" , "Dr  Wong-Lin", "Dr Kongfatt  Wong-Lin' s", "Kongfatt  Wong-Lin' s", "Dr  Wong-Lin' s"} },
            {"Dr Pratheep Yogarajah",new List<string> {"Pratheep Yogarajah" , "Dr Yogarajah", "Dr Pratheep Yogarajah' s", "Pratheep Yogarajah' s", "Dr Yogarajah' s"} },
            {"Mr Martin Doherty",new List<string> {"Martin Doherty" , "Mr Doherty", "Mr Martin Doherty' s", "Martin Doherty' s", "Mr Doherty' s"} },
            {"Ms Rosaleen Hegarty",new List<string> {"Rosaleen Hegarty" , "Ms Hegarty", "Ms Rosaleen Hegarty' s", "Rosaleen Hegarty' s", "Ms Hegarty' s"} },
            {"Ms Anne Hinds",new List<string> {"Anne Hinds" , "Ms Hinds", "Ms Anne Hinds' s", "Anne Hinds' s", "Ms Hinds' s"} },
            {"Mr Derek Woods",new List<string> {"Derek Woods" , "Mr Woods", "Mr Derek Woods' s", "Derek Woods' s", "Mr Woods' s"} }
        };
        /// <summary>
        ///struct used by the bot to build an answer 
        /// </summary>
        /// 
        //It would be great to add a functionnality in the structure to add rich contents to answer like a map for course location or lecturer picture or rich details as a card
        public struct Answer
        {
            public string value { get; private set; }
            public int fieldsNumber { get; private set; }
            public Answer(string value, int fieldsNumber)
            {
                this.value = value; this.fieldsNumber = fieldsNumber;
            }
        }

        /// <summary>
        /// ANSWER DICTIONARY, contains Answer corresponding to the key (key depend on the intent and found entities)
        /// </summary>
        public static Dictionary<string, Answer> AnswerIndex = new Dictionary<string, Answer>()
        {
            {"LecturerNameAnswer_MODID" ,new Answer( "The lecturer's name for {0} is {1}" ,1) },
            {"LecturerEmailAnswer_MODID",new Answer( " The lecturer for {0} is {1} and his email is {2}",2) },
            {"LecturerEmailAnswer_LECTNM",new Answer( "{0}'s email is {1}",1) },
            {"LecturerPhoneNumberAnswer_MODID",new Answer( "The lecturer for {0} is {1} and his phone number is +{2}",2) },
            {"LecturerPhoneNumberAnswer_LECTNM",new Answer( "{0}'s phone number is {1}",1) },
            {"LecturerOfficeAnswer_MODID",new Answer( "The lecturer for {0}’s offices is in Room {1} in the {2} building",2) },
            {"LecturerOfficeAnswer_LECTNM",new Answer( "The lecturer for {0}’s offices is in Room {1} in the {2} building",2) },
            {"LectureTimeAnswer_MODID_LECTTYPE_WNUMB",new Answer( "The course for {0} is {1} at {2} in the semester {3}",3)},
            //{"LectureTimeAnswer_MODID",new Answer( "The lecture for {0} is {1} at {2} in the semester {3}",3)},
            {"LectureLocationAnswer_MODID_LECTTYPE",new Answer( "The {1} office for {0} will be held in room {2} in the {3} building",3)},
            { "LectureLocationAnswer_MODID",new Answer( "The {1} for {0} will be held in room {2} in the {3} building",3)},
            { "LectureBeginingAnswer_MODID_WNUMB",new Answer( "Teaching for {0} starts:\n\n on {1} at {2} in {3} ",3)},
            { "ModuleOverviewAnswer_MODID",new Answer( "{0}, {1}",1)},
            //{"ScheduleAnswer_MODID_WNUMB",new Answer( "{0, {1}}",1)},
            {"ScheduleAnswer_MODID_LECTTYPE_WNUMB",new Answer( "The schedule for {0} in {1} on week {2} is \n\n {3}  ",3)},
            {"ResourceListAnswer_MODID_RESSTYPE",new Answer( "There are core text(s) for {0}: {1}, {2}. The full reading list including recommended texts is available on Blackboard and in the module handout",2)},
            {"ResourceListAnswer_MODID",new Answer( "There are core text(s) for {0}: {1}, {2}. The full reading list including recommended texts is available on Blackboard and in the module handout",2)},
            {"ModuleAssessmentOverviewAnswer_MODID",new Answer( "Here is the overview of the {0} Module : {1}",1)},
            {"ModuleAssessmentNumberAnswer_MODID",new Answer( "The coursework for {0} is team based and has {1} elements",1)},
            {"DueDateAnswer_MODID_ASSNUMB",new Answer( "The  {1} for {0} is due {2} . More detail are available on Blackboard and in the module handout",2)},
            {"DueDateAnswer_MODID",new Answer( "The {1} for {0} is due {2} . More detail are available on Blackboard and in the module handout",2)},
            {"AssessmentWeighingAnswer_MODID_ASSNUMB",new Answer( "The Weight of {0} 's {1} is  {2}",2)},
          
            {"SubmissionProcessAnswer_MODID_ASSNUMB",new Answer( "Submission process of {0}'s {1} is\n\n {2}. More detail are available on Blackboard and in the module handout.",2)},
            {"SubmissionProcessAnswer_MODID",new Answer( "The coursework for {0} is team based, {1}. More detail are available on Blackboard and in the module handout.",1)},
            {"ObjectivesAnswer_MODID_ASSNUMB",new Answer( "For {0}'s {1} you are required to :\n\n {2}",2)},
            {"ObjectivesAnswer_MODID",new Answer( "For {0}'s assignment {1} you are required to {2}",2)},
            {"WeightingAnswer_MODID",new Answer( "for {0}, weighting is : {1} . More detail are available on Blackboard and in the module handout.",1)},
            //TeamComposition ???????????????????????????????????????????????????????????????? Go to blackboard ???????????????????????
            {"TeamAssignmentOverviewAnswer_MODID",new Answer( "for {0}, {1}",1)},
            {"ExamScheduleAnswer_MODID",new Answer( "The exam for the {0} module is normally {1}.Please check Blackboard for further information.",1)},
            {"ExamFormatAnswer_MODID",new Answer( "The {0}'s exam is {1}",1)},
            {"FeedbackDateAnswer_MODID_ASSNUMB",new Answer( "Marks and feedback for the {1} of {0} are usually given in a number of different ways. {2}",2)},
           // {"FeedbackDateAnswer_MODID",new Answer( "Marks and feedback for {0} are usually given in a number of different ways. {1}",1)}
        };

        /// <summary>
        /// Corresponding key for each module page of the webchat channel
        /// </summary>
        public static Dictionary<string, string> ModuleKey = new Dictionary<string, string>
        {

            { "bs-modulebot_R5L3KvSqJF@zsPYU1zqwls","COM193" },
            { "bs-modulebot_R5L3KvSqJF@p9vArGiSib4","COM429" },
            { "bs-modulebot_R5L3KvSqJF@Xh70egLjUcc","COM631" },
            { "bs-modulebot_R5L3KvSqJF@doKw8Xk8jYY", "COM193"},
            { "bs-modulebot_R5L3KvSqJF@O4gHxHc4Btw", "COM193"}
           
        };
        public static List<string> generalSchedule = new List<string> { "general schedule", "general schedule", "general schedule" };
        /// <summary>
        /// QUERY DICTIONARY, contains all sql query, the key correspond to the intents and entities found
        /// </summary>
        /// 		queryName	"FeedbackDateQuery_MODID"	string

        public static Dictionary<string, string> QueryIndex = new Dictionary<string, string>()
        {
            {"LecturerNameQuery_MODID","Select LecturerName FROM LECTURERINFO Where LecturerID = (Select LecturerID FROM MODULEINFO where ModuleID = @ModuleID);" },
            {"LecturerEmailQuery_MODID","Select LecturerName, EmailAdress FROM LECTURERINFO Where LecturerID = (Select LecturerID FROM MODULEINFO where ModuleID = @ModuleID);" },
            {"LecturerEmailQuery_LECTNM","Select EmailAdress FROM LECTURERINFO Where LecturerName = @LecturerName;" },
            {"LecturerPhoneNumberQuery_MODID","Select LecturerName, PhoneNumber FROM LECTURERINFO Where LecturerID = (Select LecturerID FROM MODULEINFO where ModuleID = @ModuleID);" },
            {"LecturerPhoneNumberQuery_LECTNM","Select PhoneNumber FROM LECTURERINFO Where LecturerName = @LecturerName;" },
            {"LecturerOfficeQuery_MODID","select OfficeLocation,RoomBuilding from LecturerInfo inner join RoomInfo on OfficeLocation = RoomName  Where LecturerID = (Select LecturerID FROM MODULEINFO where ModuleID = @ModuleID);" },
            {"LecturerOfficeQuery_LECTNM","select OfficeLocation,RoomBuilding from LecturerInfo inner join RoomInfo on OfficeLocation = RoomName  Where LecturerName = @LecturerName;" },
            { "LectureTimeQuery_MODID_LECTTYPE_WNUMB","Select LectureDate, LectureHour, SemesterNumber FROM LECTUREINFO Where ModuleID = @ModuleID And LectureType = @LectureType;"},
            {"LectureTimeQuery_MODID_WNUMB","Select LectureDate, LectureHour, SemesterNumber FROM LECTUREINFO Where ModuleID = @ModuleID And WeekID=@Week;"},
            {"LectureLocationQuery_MODID_LECTTYPE","select LectureType,RoomName, RoomBuilding from LectureInfo inner join RoomInfo on LectureInfo.RoomID = RoomInfo.RoomID Where ModuleID = @ModuleID And LectureType = @LectureType;"},
            {"LectureLocationQuery_MODID","select LectureType,RoomName, RoomBuilding from LectureInfo inner join RoomInfo on LectureInfo.RoomID = RoomInfo.RoomID Where ModuleID = @ModuleID;"},
            //{"LectureBeginningQuery_MODID_WNUMB","Select LectureDate, LectureHour, RoomName FROM LECTUREINFO inner Join ROOMINFO on LECTUREINFO.RoomID = ROOMINFO.RoomID Where ModuleID = @ModuleID AND WeekID=@Week;"},
            {"LectureBeginingQuery_MODID_WNUMB","Select LectureDate, LectureHour, RoomName FROM LECTUREINFO inner Join ROOMINFO on LECTUREINFO.RoomID = ROOMINFO.RoomID Where ModuleID = @ModuleID And WeekID=@Week ORDER BY LectureType DESC ;"},
            {"ModuleOverviewQuery_MODID","Select ModuleOverview FROM MODULEINFO Where ModuleID = @ModuleID;"},
            {"ScheduleQuery_MODID","Select LectureInfo.LectureDate,LectureInfo.LectureHour,WeekInfo.SCHEDULEOVERVIEW, RoomInfo.RoomName FROM  LectureInfo, WeekInfo,RoomInfo Where LectureInfo.RoomID=RoomInfo.RoomID And LectureInfo.ModuleID= @ModuleID And WeekInfo.ModuleID =LectureInfo.ModuleID;"},
            {"ScheduleQuery_MODID_WNUMB","Select LectureInfo.LectureDate,LectureInfo.LectureHour,WeekInfo.SCHEDULEOVERVIEW, RoomInfo.RoomName  FROM LectureInfo, WeekInfo,RoomInfo Where LectureInfo.RoomID=RoomInfo.RoomID And LectureInfo.ModuleID= @ModuleID And WeekInfo.ModuleID =LectureInfo.ModuleID AND LectureInfo.WeekID =@Week And LectureInfo.WeekID=WeekInfo.weekNumber;"},
            {"ScheduleQuery_MODID_LECTTYPE_WNUMB","Select LectureInfo.LectureDate,LectureInfo.LectureHour, WeekInfo.SCHEDULEOVERVIEW, RoomInfo.RoomName FROM LectureInfo, WeekInfo, RoomInfo Where LectureInfo.RoomID=RoomInfo.RoomID And  LectureInfo.ModuleID= @ModuleID And WeekInfo.ModuleID =LectureInfo.ModuleID AND LectureInfo.WeekID =@Week And LectureInfo.WeekID=WeekInfo.weekNumber And LectureInfo.LectureType=@LectureType;" },
           // {"ScheduleQuery_MODID_LECTTYPE","Select LectureInfo.LectureDate,LectureInfo.LectureHour, WeekInfo.SCHEDULEOVERVIEW, RoomInfo.RoomName FROM LectureInfo, WeekInfo, RoomInfo Where LectureInfo.RoomID=RoomInfo.RoomID And  LectureInfo.ModuleID= @ModuleID And WeekInfo.ModuleID =LectureInfo.ModuleID  And LectureInfo.LectureType=@LectureType;" },
            // {"ScheduleQuery_MODID_WNUMB","Select * FROM LectureInfo Where ModuleID = @ModuleID AND WeekID = @Week AND LectureType=@LectureType"},
            {"ResourceListQuery_MODID_RESSTYPE","Select RessourceName, RessourceOverview FROM RESSOURCEINFO Where ModuleID = @ModuleID and RessourceType = @ResourceType;"},
            {"ResourceListQuery_MODID","Select RessourceName, RessourceOverview FROM RESSOURCEINFO Where ModuleID = @ModuleID;"},
            {"ModuleAssessmentOverviewQuery_MODID","Select ModuleAssessmentOverview FROM MODULEINFO Where ModuleID = @ModuleID;"},
            {"ModuleAssessmentNumberQuery_MODID","Select count (AssignmentID) FROM ASSIGNMENTINFO Where ModuleID = @ModuleID;"},
            {"DueDateQuery_MODID_ASSNUMB","Select AssessmentNumber , DueDate FROM ASSIGNMENTINFO Where AssessmentNumber = @AssessmentNumber And ModuleID = @ModuleID;"},
            {"DueDateQuery_MODID","Select AssessmentNumber , DueDate FROM ASSIGNMENTINFO Where ModuleID = @ModuleID;"},
            {"AssessmentWeighingQuery_MODID_ASSNUMB","Select AssessmentNumber, AssessmentWeighing FROM ASSIGNMENTINFO Where AssessmentNumber = @AssessmentNumber And ModuleID = @ModuleID;"},
            //{"AssessmentWeighingQuery_MODID","Select AssessmentWeighing FROM ASSIGNMENTINFO Where ModuleID = @ModuleID;"},
            {"SubmissionProcessQuery_MODID","Select SubmissionProcess FROM ASSIGNMENTINFO Where ModuleID = @ModuleID;"},
            {"SubmissionProcessQuery_MODID_ASSNUMB","Select AssessmentNumber,SubmissionProcess FROM ASSIGNMENTINFO Where AssessmentNumber = @AssessmentNumber And ModuleID = @ModuleID;"},
            {"ObjectivesQuery_MODID_ASSNUMB","Select AssessmentNumber , Objectives FROM ASSIGNMENTINFO Where AssessmentNumber = @AssessmentNumber And ModuleID = @ModuleID;"},
            {"ObjectivesQuery_MODID","Select AssessmentNumber ,Objectives FROM ASSIGNMENTINFO Where ModuleID = @ModuleID;"},
            {"WeightingQuery_MODID","Select Weighting FROM MODULEINFO Where ModuleID = @ModuleID;"},
            //TeamComposition ???????????????????????????????????????????????????????????????? Go to blackboard by yourself !!!!!!!!!!!!
            {"TeamAssignmentOverviewQuery_MODID","Select TeamAssignmentOverview FROM MODULEINFO Where ModuleID = @ModuleID;"},
            {"ExamScheduleQuery_MODID","Select ExamSchedule FROM MODULEINFO Where ModuleID = @ModuleID;"},
            {"ExamFormatQuery_MODID","Select ExamFormat FROM MODULEINFO Where ModuleID = @ModuleID;"},
            {"FeedbackDateQuery_MODID_ASSNUMB","Select AssessmentNumber, FeedbackDate FROM ASSIGNMENTINFO Where AssessmentNumber = @AssessmentNumber And ModuleID = @ModuleID;"},
            {"FeedbackDateQuery_MODID","Select FeedbackDate FROM ASSIGNMENTINFO Where ModuleID = @ModuleID;"}
        };








        /// <summary>
        /// string returned when the user try too many attemps in the Further Information dialog
        /// </summary>
        public static string TooManyAttempts = "TooManyAttempts_IkSU6kTDyNgwnGp90TmbXgRLFliHEdUxpkEFynjMeSTIZSO6FhfPxIxnYfqdIvLKqojRaIDXGv3ZVcfMsfO2EpokqxdJ8d6521Mv";
        /// <summary>
        /// string returned when the user want to exit the Further Information Dialog
        /// </summary>
        public static string exit = "exit_xlksByBnnf5gn4axcmgoRj7BrvNQu7Hpo22ucRoRslghP2sp7j9SkSn8ExOvVRbdD6gEgWAPHEmAIT8GvcQGzx3jmeeWpg1nRXIy";
        /// <summary>
        /// get corresponding key in the collection specified 
        /// </summary>
        /// <param name="collection">dictionary where the function look for corresponding</param>
        /// <param name="value">string representing the value to look for</param>
        /// <returns>returns the corresponding key of the specified collection</returns>
        public static string getCorresponding(Dictionary<string, IReadOnlyList<string>> collection, string value)
        {
            string result = null; ;

            //First try to look for the users message in the dictionnary value
            foreach (var item in collection)
            {
                if ((item.Value.Contains(value)) || (item.Key.Contains(value)))
                {
                    result = item.Key;
                }
            }
            //if not found try the users message contains a value or a key
            if (result == null)
            {
                foreach (var item in collection)
                {
                    if (value.ToLower().Contains(item.Key.ToLower()))
                    {
                        result = item.Key;
                    }
                    else
                        foreach (var item2 in item.Value)
                        {
                            if (value.ToLower().Contains(item2.ToLower()))
                                result = item.Key;
                        }
                }
            }

            //return the results
            return result;
        }
        /// <summary>
        /// get corresponding key in the collection specified , using Levenshtein distance algorithm for closest match if exact match is not found, returning also the confidence level of the closest match
        /// </summary>
        /// <param name="collection">dictionary where the function look for corresponding</param>
        /// <param name="value">string representing the value to look for</param>
        /// <param name="ExactMatch">contains the result of the function if exact match is found, else null</param>
        /// <param name="ClosestMatch">contains the result of the function if exact match is NOT found, else null</param>
        /// <param name="confidence_level">confidence level of the result, if exact match, confidence is 100</param>
        public static void getCorresponding(Dictionary<string, IReadOnlyList<string>> collection, string value, out string ExactMatch, out string ClosestMatch, out int confidence_level)
        {
            string result = null; ;

            //First try to look for the users message in the dictionnary value
            foreach (var item in collection)
            {
                if ((item.Value.Contains(value)) || (item.Key.Contains(value)))
                {
                    result = item.Key;
                }
            }
            //if not found try the users message contains a value or a key
            if (result == null)
            {
                foreach (var item in collection)
                {
                    if (value.ToLower().Contains(item.Key.ToLower()))
                    {
                        result = item.Key;
                    }
                    else
                        foreach (var item2 in item.Value)
                        {
                            if (value.ToLower().Contains(item2.ToLower()))
                                result = item.Key;
                        }
                }
            }

            //return the ExactMatch if exist of search the closest Match
            if (result != null)
            {
                ExactMatch = result;
                ClosestMatch = result;
                confidence_level = 100;
            }
            else
            {
                ExactMatch = null;
                int distance;
                FindClosest(collection, value, out ClosestMatch, out distance);
                confidence_level = (int)(System.Math.Exp(-(float)distance / 10) * 100);
            }
        }

        /// <summary>
        /// Levenshtein distance algorithm https://en.wikipedia.org/wiki/Levenshtein_distance for more information
        /// </summary>
        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1

            if (n == 0)
                return m;
            if (m == 0)
                return n;

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
        /// <summary>
        /// find the closest value in the collection when the exact match does not exist
        /// </summary>
        /// <param name="collection">dictionary where the function look for closest</param>
        /// <param name="value">string representing the value to look for</param>
        /// <param name="ClosestValue">contains the result of the function</param>
        /// <param name="distance">contains the Levenshtein distance between the closest value and the value </param>
        public static void FindClosest(Dictionary<string, IReadOnlyList<string>> collection, string value, out string ClosestValue, out int distance)
        {
            string result = String.Empty;
            int minDist = int.MaxValue;
            foreach (var list in collection)
            {
                int temp = Compute(list.Key, value);
                if (temp < minDist)
                {
                    minDist = temp;
                    result = list.Key;
                }
                foreach (var item in list.Value)
                {
                    temp = Compute(item, value);
                    if (temp < minDist)
                    {
                        minDist = temp;
                        result = list.Key;
                    }
                }
            }
            ClosestValue = result;
            distance = minDist;
        }
        /// <summary>
        /// loof for any email in the message
        /// </summary>
        public static string findEmail(string message)
        {
            string result;

            Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
            RegexOptions.IgnoreCase);
            //find items that matches with our pattern
            MatchCollection emailMatches = emailRegex.Matches(message);
            result = emailMatches[0].Value;
            return result;
        }
    }
    /// <summary>
    /// Identity of the user, with a name, an email and a module list
    /// </summary>
    public class Identity
    {
        public string GivenName { get; private set; }
        public string Email { get; private set; }
        public List<string> ModuleList { get; private set; }
        public void loginSQL(System.Data.SqlClient.SqlConnectionStringBuilder builder)
        {
            builder.DataSource = "databaseulster.database.windows.net";
            builder.UserID = "Etienne.dupuis";
            builder.Password = "Ulsterdatabase1";
            builder.InitialCatalog = "Database_Ulster";
        }
        public Identity(string Email)
        {
            this.Email = Email;
            this.GivenName = null;
            this.ModuleList = new List<string>();

            // try to find name from database using Email
            var tsql = $"SELECT StudentsName FROM STUDENTSINFO WHERE StudentsEmail = '{Email}';";
            var builder = new SqlConnectionStringBuilder();

            loginSQL(builder);
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(tsql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            GivenName = reader.GetString(0);
                    }
                }
            }

            // try to find Module from database using Students ID
            var tsql2 = $"SELECT ModuleID FROM STUDENTSMODULE WHERE StudentsID = (SELECT StudentsID FROM STUDENTSINFO WHERE StudentsEmail = '{Email}') ;";
            loginSQL(builder);
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(tsql2, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ModuleList.Add(reader.GetString(0));
                        }
                    }
                }
            }
        }
    }
