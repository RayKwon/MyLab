﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Config;
using System.Reflection;

namespace ConsoleActiveRecordTest1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Init();

                var member = new Member { Address = "add1", FirstName = "first1", Lastname = "last1" };
                var team = new Team() { Description = "desc1", Name = "name1" };
                team.Members.Add(member);
                member.Team = team;
                team.Save();


                Console.WriteLine("");
                Console.WriteLine("*******************************************************************");
                using (new SessionScope())
                {
                    foreach (var team1 in Team.FindAll())
                    {
                        Console.Write("teamID:{0}, team-memberID:{1}", 
                            team1.Id, 
                            team1.Members.First().Id);
                    }
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }

            Console.Read();
        }

        private static void Init()
        {
            IDictionary<string, string> properties = new Dictionary<string, string>();
            
            // #SQLite
            //properties.Add("connection.driver_class", "NHibernate.Driver.SQLiteDriver");
            //properties.Add("dialect", "NHibernate.Dialect.SQLiteDialect");
            //properties.Add("connection.connection_string", "Data Source=CompanyDatabase.db;Version=3");

            // #SQL Express
            properties.Add("connection.driver_class", "NHibernate.Driver.SqlClientDriver");
            properties.Add("dialect", "NHibernate.Dialect.MsSql2008Dialect");
            properties.Add("connection.connection_string", @"Data Source=.\SQL2K8R2EXP64;Initial Catalog=MyDatabase;Integrated Security=True");
            
            properties.Add("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            properties.Add("proxyfactory.factory_class",
                           "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
            properties.Add("show_sql", "true");
            properties.Add("format_sql", "true");

            var source = new InPlaceConfigurationSource();
            source.Add(typeof (ActiveRecordBase), properties);

            ActiveRecordStarter.Initialize(source, typeof (Team), typeof (Member));

            //ActiveRecordStarter.DropSchema();
            ActiveRecordStarter.CreateSchema();
        }
    }

    [ActiveRecord("Teams")]
    public class Team : ActiveRecordBase<Team>
    {
        public Team()
        {
            Members = new List<Member>();
        }

        [PrimaryKey]
        public int Id { get; set; }

        [Property("TeamName")]
        public string Name { get; set; }

        [Property]
        public string Description { get; set; }

        [HasMany(Inverse = true,
                 Lazy = true,
                 Cascade = ManyRelationCascadeEnum.AllDeleteOrphan)]
        public virtual IList<Member> Members { get; set; }
    }

    [ActiveRecord("Members")]
    public class Member : ActiveRecordBase<Member>
    {
        [PrimaryKey]
        public int Id { get; set; }

        [Property]
        public string FirstName { get; set; }

        [Property]
        public string Lastname { get; set; }

        [Property]
        public string Address { get; set; }

        [BelongsTo("TeamId")]
        public Team Team { get; set; }
    }
}
