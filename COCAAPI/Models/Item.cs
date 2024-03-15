using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace COCAAPI.Models
{
    public class ColumnValue
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }
    

    public class Item
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ColumnValue> Column_values { get; set; }
        //public List<Subitem> Subitems { get; set; }
        public Group Group { get; set; }
        //public string GroupId { get; set; }
        //public string GroupTitle { get; set; }
    }

    public class Subitem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ColumnValue> Column_values { get; set; }
    }


    public class ItemsPage
    {
        public List<Item> Items { get; set; }
    }

    public class Board
    {
        //public List<Group> Groups { get; set; }
        public ItemsPage Items_page { get; set; }
    }

    public class Group
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }

    public class Data
    {
        public List<Board> Boards { get; set; }
    }

    public class MondayApiResponse
    {
        public Data Data { get; set; }
    }
}