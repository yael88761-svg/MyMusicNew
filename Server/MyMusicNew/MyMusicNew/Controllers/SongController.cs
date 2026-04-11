using DataContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;
using Repositories.Interfaces;
using Service; // ודאי שזה ה-Namespace של ה-Tools
using Service.Dto;
using Service.Interfaces;
namespace MyMusicNew.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SongController(IService<SongDto> service,ISong<SongDto> serviceSong, Tools tools, IWebHostEnvironment env ,MusicContext context) :ControllerBase
    {
        private readonly ISong<SongDto> _serviceSong = serviceSong;
        private readonly IService<SongDto> _service = service;
        private readonly Tools _tools = tools; // הוספנו
        private readonly IWebHostEnvironment _env = env;// הוספנו
        private readonly MusicContext _context = context; // הוספנו את הצינור ל-DB

        [HttpGet("my-songs")]
        public async Task<IActionResult> GetAllSongsByUser()
        {
            try
            {
                // 1. חילוץ ה-ID מהטוקן
                var userIdFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdFromToken))
                {
                    return Unauthorized("לא נמצא מזהה משתמש בטוקן");
                }

                int currentUserId = int.Parse(userIdFromToken);

                // 2. קריאה לסרוויס המיוחד של השירים
                var userSongs = await _serviceSong.GetAll(currentUserId);

                return Ok(userSongs);
            }
            catch (Exception ex)
            {
                return BadRequest("שגיאה בשליפת השירים: " + ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var songs = await _service.GetAll();
            return Ok(songs);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var song = await _service.GetById(id);
            if (song == null)
            {
                return NotFound();
            }
            return Ok(song);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // 1. מי מנסה למחוק?
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                int currentUserId = int.Parse(userIdClaim.Value);

                // 2. נביא את השיר כדי לבדוק למי הוא שייך
                var song = await _service.GetById(id);
                if (song == null) return NotFound();

                // 3. הבדיקה הקריטית: האם ה-UserId של השיר שווה ל-ID של מי שמחובר?
                if (song.UserId != currentUserId)
                {
                    return Forbid(); // מחזיר 403 - אסור לך לגעת בשירים של אחרים
                }

                await _service.DeleteItem(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //[HttpPost]
        //public async Task<IActionResult> AddItem([FromBody] SongDto item)
        //{
        //    try
        //    {
        //        // 1. חילוץ ה-ID של המשתמש מהטוקן המאובטח
        //        // אנחנו מחפשים את ה-Claim שקראנו לו NameIdentifier ב-TokenService
        //        var userIdFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        //        if (string.IsNullOrEmpty(userIdFromToken))
        //        {
        //            return Unauthorized("לא נמצא מזהה משתמש בטוקן");
        //        }

        //        // 2. עדכון ה-DTO עם ה-ID האמיתי של המשתמש
        //        // כך אנחנו מבטיחים שהשיר יירשם על שמו של מי שמחובר כרגע
        //        item.UserId = int.Parse(userIdFromToken);

        //        // 3. שליחה ל-Service לשמירה בדאטה-בייס
        //        var addedSong = await _service.AddItem(item);

        //        return Ok(addedSong);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.ToString());
        //    }

        //}


        [HttpPost("upload-music")]
        public async Task<IActionResult> UploadMusic(IFormFile file)
        {
            try
            {
                // 1. חילוץ ה-User ID מהטוקן
                var userIdFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdFromToken)) return Unauthorized();
                int currentUserId = int.Parse(userIdFromToken);

                // 2. בדיקת הקובץ
                if (file == null || file.Length == 0) return BadRequest("לא נבחר קובץ");

                // 3. יצירת הנתיב ושמירת הקובץ ב-wwwroot/uploads
                // אנחנו בונים את הנתיב ידנית מהשורש של הפרויקט
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // 4. שימוש ב-Tools לחילוץ מידע אוטומטי (TagLib)
                var info = _tools.ExtractMetadata(filePath);

                // 5. יצירת ה-DTO ושמירה דרך ה-Service הקיים שלך
                //var newSongDto = new SongDto
                //{
                //    Title = info.Title,
                //    Artist = info.Artist,
                //    FilePath = "/uploads/" + uniqueFileName,
                //    UserId = currentUserId,
                //    Genre = "General" // ברירת מחדל
                //};

                //var addedSong = await _service.AddItem(newSongDto);

                // 6. הפעלת ה-AI לניתוח מאפיינים חכמים (אופציונלי)
                //var features = await _tools.GetAudioFeaturesFromAI(addedSong.Title, addedSong.Artist, addedSong.SongId);
                // כאן אפשר להוסיף שמירה של ה-features ל-DB אם יש לך סרוויס מתאים
                // 6. הפעלת ה-AI לניתוח מאפיינים חכמים
                //var features = await _tools.GetAudioFeaturesFromAI(addedSong.Title, addedSong.Artist, addedSong.SongId);

                //if (features != null)
                //{
                //    // פקודה שאומרת ל-Entity Framework: "תוסיף את האובייקט הזה לטבלת המאפיינים"
                //    _context.AudioFeatures.Add(features);

                //    // פקודה קריטית: "תשמור את כל השינויים שביצענו עכשיו ב-Database"
                //    await _context.SaveChangesAsync();
                //}



                // 5. יצירת ה-DTO ושמירה
                var newSongDto = new SongDto
                {
                    Title = info.Title,
                    Artist = info.Artist,
                    FilePath = "/uploads/" + uniqueFileName,
                    UserId = currentUserId,
                    Genre = "General"
                };

                // שמירת השיר ב-DB - חובה לשמור כאן כדי שיהיה לו ID!
                var addedSong = await _service.AddItem(newSongDto);
                // ודאי שה-Service שלך באמת עושה SaveChanges בפנים. אם לא, תוסיפי:
                // await _context.SaveChangesAsync(); 

                // 6. הפעלת ה-AI
                //var features = await _tools.GetAudioFeaturesFromAI(addedSong.Title, addedSong.Artist, addedSong.SongId);

                //if (features != null)
                //{
                //    // ודאי שה-SongId ב-features תואם ל-ID שנוצר עכשיו
                //    features.SongId = addedSong.SongId;

                //    _context.AudioFeatures.Add(features);
                //    await _context.SaveChangesAsync(); // שמירה שנייה עבור ה-Features
                //}
                // 6. הפעלת ה-AI לניתוח מאפיינים חכמים
                try
                {
                    // אנחנו שולחים את המידע ל-AI. 
                    // הוספתי בדיקה ש-addedSong לא ריק לפני הקריאה
                    if (addedSong != null)
                    {
                        var features = await _tools.GetAudioFeaturesFromAI(addedSong.Title, addedSong.Artist, addedSong.SongId);

                        if (features != null)
                        {
                            // קישור ה-ID של השיר שנוצר לנתוני ה-AI
                            features.SongId = addedSong.SongId;

                            _context.AudioFeatures.Add(features);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception aiEx)
                {
                    // אם ה-AI נכשל, אנחנו לא רוצים שכל ההעלאה תיפול.
                    // פשוט נדפיס למערכת שהניתוח נכשל אבל השיר נשמר.
                    Console.WriteLine("AI Analysis failed: " + aiEx.Message);
                }

                return Ok(new { message = "הקובץ הועלה בהצלחה!", data = addedSong });



                return Ok(new { message = "הקובץ הועלה ונותח!", data = addedSong });
            }
            catch (Exception ex)
            {
                // מחלץ את השגיאה הכי עמוקה (שם נמצאת הסיבה האמיתית)
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : "אין פירוט נוסף";

                // מדפיס למסך גם את השגיאה הכללית וגם את הפירוט של ה-Database
                return BadRequest(new
                {
                    error = "שגיאה בשמירה למסד הנתונים",
                    details = ex.Message,
                    databaseError = innerMessage
                });
            }
        }

    }
}
