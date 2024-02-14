using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace COCAAPI.Models
{
    public class ColumnValue
    {
        public string id { get; set; }
        public string text { get; set; }
    }

    public class Item
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ColumnValue> Column_values { get; set; }
        public string GroupId { get; set; }
        public string GroupTitle { get; set; }
    }

    public class ItemsPage
    {
        public List<Item> Items { get; set; }
    }

    public class Board
    {
        public List<Group> Groups { get; set; }
    }

    public class Group
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public ItemsPage Items_page { get; set; }
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