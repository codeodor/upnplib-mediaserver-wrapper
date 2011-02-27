using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using upnplib_mediaserver_wrapper; 

namespace example_upnp_mediaserver_browser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Out.WriteLine("Use 'ls' to list the contents of a content directory.");
            Console.Out.WriteLine("Use 'cd X' to change into a new content directory. X is the number output by the 'ls' command.");
            Console.Out.WriteLine("Use 'q' to quit.\n");
            Console.Out.WriteLine("Wait for a few seconds while the list of media servers populates below:\n");

            string command = "ls";
            List<MediaServer> servers = MediaServer.All();
            int serverIndex = -1;
            int childIndex;
            List<ContentDirectory> stack = new List<ContentDirectory>();

            while (command != "q")
            {
                string[] commandArgs = command.Split(' ');
                command = commandArgs[0];
                string obj = null;
                if (commandArgs.Length > 1)
                    obj = commandArgs[1];

                switch (command)
                {
                    case "ls":
                        int i = 0;
                        if (stack.Count == 0)
                        {
                            //servers = MediaServer.All();
                            foreach (MediaServer s in servers)
                            {
                                Console.Out.WriteLine(i + ": " + s.Title);
                                i++;
                            }
                        }
                        else
                        {
                            foreach (ContentDirectory cd in stack[stack.Count - 1].Children)
                            {
                                Console.Out.WriteLine(i + ": " + cd.Title);
                                i++;
                            }
                        }
                        break;
                    case "cd":
                        if (obj == "..")
                            if (stack.Count > 0)
                                stack.RemoveAt(stack.Count - 1);
                            else
                                break;
                        else
                        {
                            childIndex = int.Parse(obj);
                            if (stack.Count == 0 && childIndex < servers.Count && childIndex >= 0)
                                stack.Add(servers[childIndex].ContentDirectory);
                            else if (stack.Count == 0)
                                Console.Out.WriteLine("Not a valid server.");
                            else
                                stack.Add(stack[stack.Count - 1].Children[childIndex]);
                        }
                        break;
                    default:
                        Console.Out.WriteLine("Invalid command. Try 'ls' to list the current options then 'cd X' into one of them or 'cd ..' to back up a directory.");
                        break;

                }

                Console.Out.Write("\nEnter command (q to quit): ");
                command = Console.In.ReadLine();
            }
        }
    }
}
