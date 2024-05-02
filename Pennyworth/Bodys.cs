using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pennyworth
{
    public class Bodys
    {
        public class Response
        {
            public string DocEntry { get; set; }
            public string DocNum { get; set; }
            public List<Conflicts> conflicts { get; set; }
            public List<Products> products { get; set; }
        }

        public class Conflicts
        {
            public bool Problems { get; set; }
            public string Description { get; set; }
        }

        public class Products
        {
            public string ItemName { get; set; }
            public string ItemCode { get; set; }
            public string BatchNum { get; set; }
            public string ExpDate { get; set; }
            public string CodeBar { get; set; }
            public string Ubications { get; set; }
        }

        public class Ubications
        {
            public string ubication { get; set; }
        }

        public class GetItem
        {
            public string whsCode { get; set; }
            public string sku { get; set; }
        }

        public class InventoryCountings
        {
            public string Reference2 { get; set; }
            public string CountingType { get; set; }
            public List<InventoryCountingLines> InventoryCountingLines { get; set; }
        }

        public class InventoryCountingLines
        {
            public int LineNumber { get; set; }
            public string ItemCode { get; set; }
            public string Freeze { get; set; }
            public string WarehouseCode { get; set; }
            public int BinEntry { get; set; }
            public string Counted { get; set; }
            public double CountedQuantity { get; set; }
            public List<InventoryCountingBatchNumbers> InventoryCountingBatchNumbers { get; set; }
        }

        public class InventoryCountingBatchNumbers
        {
            public int Quantity { get; set; }
            public string BatchNumber { get; set; }
        }

        public class InventoryPostings
        {
            public string JournalRemark { get; set; }

            public List<InventoryPostingLines> InventoryPostingLines { get; set; }
        }

        public class InventoryPostingLines
        {
            public string ItemCode { get; set; }
            public string WarehouseCode { get; set; }
            public int BinEntry { get; set; }
            public int CountedQuantity { get; set; }
            public int BaseEntry { get; set; }
            public int BaseLine { get; set; }
            public string BaseReference { get; set; }
            public List<InventoryPostingBatchNumbers> InventoryPostingBatchNumbers { get; set; }
        }

        public class InventoryPostingBatchNumbers
        {
            public int Quantity { get; set;}
            public string BatchNumber { get; set; }
            //public int BaseLineNumber { get; set; }
        }
    }
}