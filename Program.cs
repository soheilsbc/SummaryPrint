using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections;
using System.Text.RegularExpressions;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //This counter is declared to keep track of number of records to show after every 1000 records
            int counter = 0;

            //These arraylists are declared for the calculation part for mean and median
            ArrayList UserList = new ArrayList();
            ArrayList FriendsList = new ArrayList();
            ArrayList AgeList = new ArrayList();
            ArrayList BalanceList = new ArrayList();
            ArrayList FemaleActive = new ArrayList();



            //Please define the path to the JSON file
            StreamReader r = new StreamReader("G:\\My Drive\\Job\\Aiseraa\\drive-download-20181019T180802Z-001\\data\\data\\users-2.json");

            //This part stream the data
            string json = r.ReadToEnd();
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            using (MemoryStream stream = new MemoryStream(bytes))
            using (StreamReader sr = new StreamReader(stream))
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                reader.SupportMultipleContent = true;
                var serializer = new JsonSerializer();
                while (reader.Read())
                { 
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        Itemclass c = serializer.Deserialize<Itemclass>(reader);
                        counter++;

                        //c.registered is used to access to the users that are registered in each year then it is sorted to find the median
                        UserList.Add((c.registered).Substring(0, 4) + c.name);
                        UserList.Sort();

                        //It counts the number of friends for each user then sort it to find the median
                        FriendsList.Add(Regex.Matches((c.friends).ToString(), "name").Count);
                        FriendsList.Sort();

                        //Access the age of users and sort them to find the median
                        AgeList.Add(c.age);
                        AgeList.Sort();

                        //Access the balance for computing the mean
                        BalanceList.Add(c.balance.Replace("$","").Replace(",",""));
                        
                        //Access the number of unread messages for active females 
                        if (c.gender == "female" && c.isActive == true)
                        {
                            string tempgreeting = c.greeting;
                            int startunread = tempgreeting.IndexOf("have ");
                            tempgreeting = tempgreeting.Substring(startunread+5);
                            int endunread = tempgreeting.IndexOf(" unread");
                            tempgreeting = tempgreeting.Substring(0, endunread);
                            FemaleActive.Add(tempgreeting);
                        }

                        //Show the summary after every 1000 records
                        if (counter == 1000)
                        {
                            //Show registered users in each year
                            RegisteredUsers(UserList);

                            //Show median for number of friends
                            MedianNumberFriends(FriendsList);

                            //Show median for the age
                            MedianAgeUsers(AgeList);

                            //Show mean for balance amount
                            MeanBalance(BalanceList);

                            //Show mean for number of unread message for active female users
                            MeanUnread(FemaleActive);

                            //Clear the arraylists in order to make sure only 1000 records are using in the computation
                            AgeList.Clear();
                            UserList.Clear();
                            BalanceList.Clear();
                            FriendsList.Clear();
                            FemaleActive.Clear();

                            //Reset the counter to show the summary for the next 1000 records
                            counter = 0;

                            //Draw a line to separate each 1000 records
                            Console.WriteLine("---------------------------------");
                        }
                    }  
                }
            }
            Console.ReadLine();
        }

        private static void MeanUnread(ArrayList femaleActive)
        {
            int sum = 0;
            for (int i = 0; i < femaleActive.Count; i++)
            {
                sum += Convert.ToInt32(femaleActive[i]);
            }
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("The Mean for number of Unread messages for Active females is " + sum / femaleActive.Count);
            Console.ResetColor();
        }

        private static void MeanBalance(ArrayList balanceList)
        {
            double meanbalance = 0;
            for (int i = 0; i < balanceList.Count; i++)
            {
                meanbalance += Convert.ToDouble(balanceList[i]);
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("The Mean balance amount is " + String.Format("{0:C2}", (meanbalance / balanceList.Count)));
            Console.ResetColor();
        }

        private static void MedianAgeUsers(ArrayList ageList)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("The Median age is " + ageList[ageList.Count / 2]);
            Console.ResetColor();
        }

        private static void MedianNumberFriends(ArrayList friendsList)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The Median for the number of friends is " + friendsList[friendsList.Count / 2]);
            Console.ResetColor();
        }

        private static void RegisteredUsers(ArrayList userList)
        {
            string temp = userList[0].ToString();
            temp = temp.Substring(0, 4);
            Console.WriteLine("Registered users in the year " + temp + " :");
            foreach (var item in userList)
            {
                if (item.ToString().Substring(0, 4) != temp)
                {
                    Console.WriteLine("Registered users in the year " + item.ToString().Substring(0, 4) + " :");
                    temp = item.ToString().Substring(0, 4);
                }
                Console.WriteLine(item.ToString().Substring(4));
            }
        }

        public class Itemclass
        {
            public object friends { get; set; }
            public string greeting { get; set; }
            public string gender { get; set; }
            public Boolean isActive { get; set; }
            public string name { get; set; }  
            public string registered { get; set; }
            public string balance { get; set; }
            public int age { get; set; }
        }
    }
}
