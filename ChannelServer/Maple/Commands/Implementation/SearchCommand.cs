using System.Collections.Generic;
using Loki.Maple.Characters;

namespace Loki.Maple.Commands.Implementation
{
    class SearchCommand : Command
    {
        public override bool IsRestricted { get { return true; } }
        public override string Name { get { return "search"; } }
        public override string Parameters { get { return "[ -item | -map | -mob | -npc | -quest ] label"; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length < 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                string query;

                if (args[0].StartsWith("-"))
                {
                    query = CombineArgs(args, 1).ToLower();
                }
                else
                {
                    query = CombineArgs(args).ToLower();
                }

                if (query.Length < 2)
                {
                    caller.Notify("   Please enter at least 2 characters.");
                }
                else
                {
                    bool hasResult = false;

                    caller.Notify("[Results]");

                    if (args[0].ToLower() == "-item")
                    {
                        foreach (KeyValuePair<string, int> item in Strings.Items)
                        {
                            if (item.Key.ToLower().Contains(query))
                            {
                                caller.Notify(string.Format("   -{0}: {1}", item.Key, item.Value));
                                hasResult = true;
                            }
                        }
                    }
                    else if (args[0].ToLower() == "-map")
                    {
                        foreach (KeyValuePair<string, int> map in Strings.Maps)
                        {
                            if (map.Key.ToLower().Contains(query))
                            {
                                caller.Notify(string.Format("   -{0}: {1}", map.Key, map.Value));
                                hasResult = true;
                            }
                        }
                    }
                    else if (args[0].ToLower() == "-mob")
                    {
                        foreach (KeyValuePair<string, int> mob in Strings.Mobs)
                        {
                            if (mob.Key.ToLower().Contains(query))
                            {
                                caller.Notify(string.Format("   -{0}: {1}", mob.Key, mob.Value));
                                hasResult = true;
                            }
                        }
                    }
                    else if (args[0].ToLower() == "-npc")
                    {
                        foreach (KeyValuePair<string, int> npc in Strings.Npcs)
                        {
                            if (npc.Key.ToLower().Contains(query))
                            {
                                caller.Notify(string.Format("   -{0}: {1}", npc.Key, npc.Value));
                                hasResult = true;
                            }
                        }
                    }
                    else if (args[0].ToLower() == "-quest")
                    {
                        foreach (KeyValuePair<string, int> quest in Strings.Quests)
                        {
                            if (quest.Key.ToLower().Contains(query))
                            {
                                caller.Notify(string.Format("   -{0}: {1}", quest.Key, quest.Value));
                                hasResult = true;
                            }
                        }
                    }
                    else
                    {
                        foreach (KeyValuePair<string, int> item in Strings.Items)
                        {
                            if (item.Key.ToLower().Contains(query))
                            {
                                caller.Notify(string.Format("   -Item: {0}: {1}", item.Key, item.Value));
                                hasResult = true;
                            }
                        }

                        foreach (KeyValuePair<string, int> map in Strings.Maps)
                        {
                            if (map.Key.ToLower().Contains(query))
                            {
                                caller.Notify(string.Format("   -Map: {0}: {1}", map.Key, map.Value));
                                hasResult = true;
                            }
                        }

                        foreach (KeyValuePair<string, int> mob in Strings.Mobs)
                        {
                            if (mob.Key.ToLower().Contains(query))
                            {
                                caller.Notify(string.Format("   -Mob: {0}: {1}", mob.Key, mob.Value));
                                hasResult = true;
                            }
                        }

                        foreach (KeyValuePair<string, int> npc in Strings.Npcs)
                        {
                            if (npc.Key.ToLower().Contains(query))
                            {
                                caller.Notify(string.Format("   -Npc: {0}: {1}", npc.Key, npc.Value));
                                hasResult = true;
                            }
                        }

                        foreach (KeyValuePair<string, int> quest in Strings.Quests)
                        {
                            if (quest.Key.ToLower().Contains(query))
                            {
                                caller.Notify(string.Format("   -Quest: {0}: {1}", quest.Key, quest.Value));
                                hasResult = true;
                            }
                        }
                    }

                    if (!hasResult)
                    {
                        caller.Notify("   No result found.");
                    }
                }
            }
        }
    }
}