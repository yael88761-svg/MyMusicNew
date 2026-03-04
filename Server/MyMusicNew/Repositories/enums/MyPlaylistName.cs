using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.enums
{
    public enum MyPlaylistsNames
    {
        // אווירה ורגש
        SimchaAndDance = 1,    // שמחה וריקודים
        ChillAndRelax = 2,     // שירי רגש ונשמה
        VocalOnly = 3,         // ווקאלי (לימי ספירה ובין המצרים)
        Kumzits = 4,           // קומזיץ

        // זמנים ומועדים
        ShabbatPrep = 5,       // לכבוד שבת קודש
        MotzaeiShabbat = 6,    // מוצאי שבת
        WeddingHits = 8,       // להיטי חתונות

        // חגים ומועדים (מפורט)
        ElulAndHighHolidays = 20, // אלול וימים נוראים
        Sukkot = 21,              // סוכות ושמחת בית השואבה
        Chanukah = 22,            // חנוכה
        Purim = 23,               // פורים
        Pesach = 24,              // פסח
        Shavuot = 25,             // שבועות

        // לימוד וריכוז
        Concentration = 9,     // מוזיקה לריכוז ולימוד
        Instrumental = 10,     // אינסטרומנטלי (ללא מילים)

        // כללי
        NewReleases = 11,      // סינגלים חדשים
        Oldies = 12            // נוסטלגיה

    }

    public class MyPlaylistName
    {
    }
}
