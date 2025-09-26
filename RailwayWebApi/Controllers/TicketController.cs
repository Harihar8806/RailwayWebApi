using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using RailwayWebApi.Data;
using RailwayWebApi.Models;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography.Pkcs;
using System.Text.Json;
using static System.Collections.Specialized.BitVector32;

namespace RailwayWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ApplicationDbContext _context;

        //public TicketController(IConfiguration configuration)
        //{
        //    _connectionString = configuration.GetConnectionString("OracleDb");
        //}

        public TicketController(ApplicationDbContext context)
        {

            _context = context;
        }

        //[HttpPost]
        //public IActionResult Bookticket([FromBody] TicketBookingRequest request)
        //{
        //    try
        //    {
        //        using var con = new OracleConnection(_connectionString);
        //        using var cmd = new OracleCommand("BOOKING_PKG.BOOK_MULTIPLE_TICKETS", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.BindByName = true;
        //        cmd.Parameters.Add("P_TRAINID", OracleDbType.Int32).Value = request.TrainId;
        //        cmd.Parameters.Add("V_SOURCE_STATION", OracleDbType.Int32).Value = request.FromStation;
        //        cmd.Parameters.Add("V_DESTINATION_STATION", OracleDbType.Int32).Value = request.ToStation;
        //        cmd.Parameters.Add("P_BOOKING_DATE", OracleDbType.Date).Value = request.BookingDate;
        //        cmd.Parameters.Add("P_COACHTYPE", OracleDbType.Varchar2).Value = request.CoachType;
        //        cmd.Parameters.Add("P_QUOTANAME", OracleDbType.Varchar2).Value = request.QuotaName;
        //        var passengerParam = new OracleParameter("P_PASSENGER_DETAILS", OracleDbType.Object);
        //        passengerParam.UdtTypeName = "EMS_ASP.passenger_tab";
        //        passengerParam.Value = request.PassengerDetail.Select(p => new object[]
        //        {  p.FULLNAME,p.AGE,p.GENDER,p.PHONE,p.EMAIL}
        //        ).ToArray();
        //        cmd.Parameters.Add(passengerParam);
        //        var messageParam = new OracleParameter("V_MESSAGE", OracleDbType.Varchar2, 4000)
        //        {
        //            Direction = ParameterDirection.Output
        //        };
        //        cmd.Parameters.Add(messageParam);
        //        con.Open();
        //        cmd.ExecuteNonQuery();
        //        string resultMessage = messageParam.Value?.ToString();
        //        return Ok(new { Message = resultMessage });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}



        [HttpPost("BookTicket")]
        public async Task<IActionResult> BookTicket([FromBody] TicketBookingRequest request)
        {
            try
            {
                var passengersJson = JsonSerializer.Serialize(request.PassengerDetail);
                var errorParam = new OracleParameter("V_MESSAGE", OracleDbType.Varchar2, 4000)
                {
                    Direction = System.Data.ParameterDirection.Output
                };

                var passengerParam = new OracleParameter("P_PASSENGER_DETAILS", OracleDbType.Clob, passengersJson, ParameterDirection.Input);

                if (!DateTime.TryParseExact(request.BookingDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime BookingDate))
                {
                    return BadRequest(new { Message = "Invalid Date format.Use YYYY-MM-DD" });
                }

                await _context.Database.ExecuteSqlRawAsync(
                    @"Begin BOOK_MULTIPLE_TICKETS_1
                (:P_TRAINID,:V_SOURCE_STATION,:V_DESTINATION_STATION,:P_BOOKING_DATE,:P_COACHTYPE,:P_QUOTANAME,:P_PASSENGER_DETAILS,:V_MESSAGE) ;END;",

                new OracleParameter("P_TRAINID", request.TrainId),
                new OracleParameter("V_SOURCE_STATION", request.FromStation),
                new OracleParameter("V_DESTINATION_STATION", request.ToStation),
                new OracleParameter("P_BOOKING_DATE", request.BookingDate),
                new OracleParameter("P_COACHTYPE", request.CoachType),
                new OracleParameter("P_QUOTANAME", request.QuotaName),
                passengerParam,
                errorParam);

                string errorMessage = errorParam.Value.ToString();
                if (errorMessage == "Success")
                    return Ok(new { Message = "Booking Successful" });
                else return BadRequest(new { Message = errorMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("betweenStations")]
        public async Task<IActionResult> GetTrains(int fromstation, int tostation)
        {
            try
            {
                var pFrom = new OracleParameter("P_SOURCE", OracleDbType.Int32, fromstation, ParameterDirection.Input);
                var pTo = new OracleParameter("P_DESTINATION", OracleDbType.Int32, tostation, ParameterDirection.Input);
                var pCursor = new OracleParameter("P_OUTPUT", OracleDbType.RefCursor, ParameterDirection.Output);

                var trains = await _context.Set<TrainBetweenStation>()
                    .FromSqlRaw("BEGIN SP_TRAIN_SOUR_DESTIN(:P_SOURCE,:P_DESTINATION,:P_OUTPUT) ; END;",
                    pFrom, pTo, pCursor).ToListAsync();

                return Ok(trains);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("Calculate")]
        public async Task<IActionResult> GetTrainfare(int trainnumber, int sourcestation, int tostation, string coachtype)
        {
            try
            {
                var ptrainnumber = new OracleParameter("P_TRAINNO", OracleDbType.Int32, trainnumber, ParameterDirection.Input);
                var psourcestation = new OracleParameter("P_SOURCESTATION", OracleDbType.Int32, sourcestation, ParameterDirection.Input);
                var pdestinationstation = new OracleParameter("P_DESTINATION", OracleDbType.Int32, tostation, ParameterDirection.Input);
                var pcoachtype = new OracleParameter("P_COACH_TYPE", OracleDbType.Varchar2, coachtype, ParameterDirection.Input);
                var pCursor = new OracleParameter("P_OUTPUT", OracleDbType.RefCursor, ParameterDirection.Output);

                var trainfare = await _context.Set<CalculateFare>()
                    .FromSqlRaw("BEGIN SP_GET_TRAIN_FARE(:P_TRAINNO,:P_SOURCESTATION,:P_DESTINATION,:P_COACH_TYPE,:P_OUTPUT) ; END;",
                    ptrainnumber, psourcestation, pdestinationstation, pcoachtype, pCursor).ToListAsync();
                return Ok(trainfare);
            }
            catch (Exception ex)
            {
                return BadRequest($"error :{ex.Message}");
            }
        }

        [HttpPost("AddStation")]
        public async Task<IActionResult> AddStation([FromBody] Station station)
        {
            if (station == null)
            {
                return BadRequest("Invalid Station Details");
            }

            var exisitinhStation = await _context.STATIONS
                                 .FirstOrDefaultAsync(s => s.STATIONID == station.STATIONID);

            if (exisitinhStation != null)
            {
                return Conflict("Station With This Name Already Exists.");
            }

            _context.STATIONS.Add(station);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetStationById), new { id = station.STATIONID }, station);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetStationById(int Id)
        {
            var station=await _context.STATIONS.FindAsync(Id);
            if (station == null)
                return NotFound();
            return Ok(station);
        }

        [HttpPut("UpdateStation/{stationcode}")]
        public async Task<IActionResult> UpdateStation(string stationcode, [FromBody] Station station)
        {
            if (station == null || string.IsNullOrWhiteSpace(stationcode) )
                return BadRequest("Station Data Mismatch");

            var exisitingstation =await  _context.STATIONS.FirstOrDefaultAsync(stn=>stn.STATIONCODE == stationcode);
            if(exisitingstation==null)
            {
                return NotFound("station Not Found");
            }

            exisitingstation.STATIONNAME = station.STATIONNAME;
            _context.STATIONS.Update(exisitingstation);
            await _context.SaveChangesAsync();
            return Ok(new {message="Station update Successfully",station= exisitingstation });
        }

        [HttpPost("AddTrain")]
        public async Task<IActionResult> AddTrain([FromBody]Train train)
        {
            if (train == null) return BadRequest("Train not Found");

            var existingtrain = await _context.Trains.FirstOrDefaultAsync(s => s.TRAINID == train.TRAINID);
            if (existingtrain != null)
            {
                return Conflict("Train With This Name Already Exists.");  
            }
            _context.Trains.Add(train);
            await _context.SaveChangesAsync();
            return CreatedAtAction(actionName: "GetTrainByNumber",controllerName: "Account",
                                    routeValues: new { trainNumber = train.TRAINNUMBER},value: train);
        }

        [HttpPut("UpdateTrain/{trainNumber}")]
        public async Task<IActionResult> UpdateTrain(int trainNumber, [FromBody] Train train)
        {
            if (train == null || trainNumber==null)
                return BadRequest("Train Data Mismatch");

            var exisitingstation = await _context.Trains.FirstOrDefaultAsync(stn => stn.TRAINNUMBER == trainNumber);
            if (exisitingstation == null)
            {
                return NotFound("Train Not Found");
            }

            exisitingstation.SOURCESTATIONID = train.SOURCESTATIONID;
            exisitingstation.SOURCESTATIONNAME = train.SOURCESTATIONNAME;
            exisitingstation.DESTNATIONSTATIONID = train.DESTNATIONSTATIONID;
            exisitingstation.DESTNATIONSTATIONNAME = train.DESTNATIONSTATIONNAME;
            exisitingstation.TOTALDISTANCE = train.TOTALDISTANCE;
            _context.Trains.Update(exisitingstation);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Train update Successfully", station = exisitingstation });
        }

        [HttpDelete("RemoveTrain{trainnumber}")]
        public async Task<IActionResult> RemoveTrain(int trainnumber, [FromBody] Train train)
        {
            if (train==null)
            {
                return BadRequest("The Train is not present");
            }
            var exisistingTrain = await _context.Trains.FirstOrDefaultAsync(s=>s.TRAINNUMBER == trainnumber);
            if (exisistingTrain == null)
            {
                return NotFound("Train Not Found");
            }
            _context.Trains.Remove(exisistingTrain);
            await _context.SaveChangesAsync();
            return Ok(new { message="Train Deleted Successfully"});
        }

        [HttpPost("AddRoute")]
        public async Task<IActionResult> AddRoute([FromBody] List<TrainRouteDTO> trainroute)
        {
            if (trainroute == null||trainroute.Count==0)
                return BadRequest("Trainroute Not Found");

           

            var trainRoutes = trainroute.Select(r=>new TrainRoutes
            {
                  TRAINID=r.TRAINID,
                 STATIONID=r.STATIONID,
                 STATIONORDER=r.STATIONORDER,
                 SCHEDULEARRIVAL=r.SCHEDULEARRIVAL, 
                 SCHEDULEDEPARTURE=r.SCHEDULEDEPARTURE,
                 PLATFORMNUMBER=r.PLATFORMNUMBER,
                 DAY=r.DAY,
                 DISTANCE=r.DISTANCE
            }).ToList();
            

           _context.TrainRoutes.AddRange(trainRoutes);
         await _context.SaveChangesAsync();
       
            return Ok( new { message = "Train routes inserted Successfully", routes=trainroute });

        }

        [HttpGet("GetTrainRoute{trainid}")]
        public async Task<IActionResult> GetTrainRoute(int trainid)
        {
            var exisitingroute = await _context.TrainRoutes.Where(s=>s.TRAINID == trainid)
                                    .ToListAsync();

            if (exisitingroute == null) 
            { return NotFound("The trainroutes is not available");
            };

                var routesDtos = new List<TrainRoutes>();

                foreach(var routes in exisitingroute)
                {
                    routesDtos.Add(new TrainRoutes
                    {
                        TRAINID = routes.TRAINID,
                        STATIONID=routes.STATIONID,
                        STATIONORDER = routes.STATIONORDER,
                        SCHEDULEARRIVAL=routes.SCHEDULEARRIVAL,
                        SCHEDULEDEPARTURE=routes.SCHEDULEDEPARTURE,
                        PLATFORMNUMBER=routes.PLATFORMNUMBER,
                        DAY=routes.DAY,
                        DISTANCE=routes.DISTANCE
                    });

                }
                return Ok(routesDtos);
        }

        [HttpGet("InsertStationBetween")]
        public async Task<IActionResult> StationBetween(int trainid,int afterorder,int stationid,DateTime arrivaltime,
                                        DateTime departuretime,int platform,int day, int distance)
        {
            try
            {
                var ptrainid = new OracleParameter("P_TRAINID", OracleDbType.Int32) { Value = trainid };
                var pafterorder = new OracleParameter("P_AFTER_ORDER", OracleDbType.Int32) { Value = afterorder };
                var pstationid = new OracleParameter("P_STATIONID", OracleDbType.Int32) { Value = stationid };
                var parrivaltime = new OracleParameter("P_ARRIVAL_TIME", OracleDbType.TimeStamp) { Value = arrivaltime };
                var pdeparturetime = new OracleParameter("P_DEPARTURE_TIME", OracleDbType.TimeStamp) { Value = departuretime };
                var pplatform = new OracleParameter("P_PLATFORMNUMBER", OracleDbType.Int32) { Value = platform };
                var pday = new OracleParameter("P_DAY", OracleDbType.Int32) { Value = day };
                var pdistance = new OracleParameter("P_DISTANCE", OracleDbType.Int32) { Value = distance };

                var pmessage = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 200)
                {
                    Direction = ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "BEGIN SP_INSERT_STATION_BETWEEN(:P_TRAINID,:P_AFTER_ORDER,:P_STATIONID,:P_ARRIVAL_TIME,:P_DEPARTURE_TIME,:P_PLATFORMNUMBER,:P_DAY,:P_DISTANCE,:P_MESSAGE); END;",
                               ptrainid, pafterorder, pstationid, parrivaltime, pdeparturetime, pplatform, pday, pdistance, pmessage);
                return Ok(pmessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("AddCoaches")]
        public async Task<IActionResult> AddCoaches([FromBody] List<Coaches> coaches)
        {
            if (coaches == null || coaches.Count == 0)
                return BadRequest("Trainroute Not Found");



            var traincoaches = coaches.Select(r => new Coaches
            {
                TRAINID= r.TRAINID,
                COACHPOSITION= r.COACHPOSITION,
                COACHNUMBER=r.COACHNUMBER,
                COACHTYPE=r.COACHTYPE,
                TOTALSEATES=r.TOTALSEATES

            }).ToList();


            _context.COACHES.AddRange(traincoaches);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Train coaches inserted Successfully", routes = traincoaches });

        }

        [HttpPut("UpdateCoaches")]
        public async Task<IActionResult> UpdateCoaches(int trainid,int conachesposition, [FromBody] Coaches coache)
        {
            if(trainid==null|| conachesposition==null)
            {
                return BadRequest("Pleaase enter Traind and Coacheposition");
            }

            var existingcoachs = await _context.COACHES.Where(x=>x.TRAINID==trainid && x.COACHPOSITION== conachesposition).FirstOrDefaultAsync();

            if(existingcoachs==null)
            {
                return BadRequest("The TraindID is not present in the System");
            }
            existingcoachs.COACHNUMBER = coache.COACHNUMBER;
            existingcoachs.COACHTYPE= coache.COACHTYPE;
            existingcoachs.TOTALSEATES= coache.TOTALSEATES;
            

            _context.COACHES.Update(existingcoachs);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Train coaches has been updated Successfully for Trainid {trainid} AND Coachposition {conachesposition}" });
        }

    }
}



