using System.Collections.Generic;
using System.Web.Mvc;
using TallyJ.Code;
using TallyJ.Code.Session;
using TallyJ.Models;
using System.Linq;

namespace TallyJ.Controllers
{
  public class BallotsController : BaseController
  {
    ///// <Summary>switch to location l, show ballot b</Summary>
    //public ActionResult Index(int l, int b)
    //{
    //  if (UserSession.CurrentLocation == null || l != UserSession.CurrentLocation.C_RowId)
    //  {
    //    // switch to location, if allowed
    //    var switched = new ComputerModel().AddCurrentComputerIntoLocation(l);
    //    if (!switched)
    //    {
    //      return RedirectToAction("ChooseElection", "Dashboard");
    //    }
    //  }

    //  return Index(b);
    //}

    ///// <Summary>Show the desired ballot</Summary>
    //public ActionResult Index(int b)
    //{
    //  SessionKey.CurrentBallotId.SetInSession(b);
    //  return Index();
    //}

    public ActionResult Index()
    {
      var locationId = Request.QueryString["l"].AsInt();
      if (locationId != 0 && (UserSession.CurrentLocation == null || locationId != UserSession.CurrentLocation.C_RowId))
      {
        // switch to location, if allowed
        var switched = new ComputerModel().AddCurrentComputerIntoLocation(locationId);
        if (!switched)
        {
          return RedirectToAction("ChooseElection", "Dashboard");
        }
      }

      var isSingle = UserSession.CurrentElection.IsSingleNameElection.AsBool();
      var ballotModel = CurrentBallotModel;

      var ballotId = Request.QueryString["b"].AsInt();
      if (ballotId == 0)
      {
        if (isSingle)
        {
          ballotModel.GetCurrentBallotInfo();
        }
      }
      else
      {
        ballotModel.SetAsCurrentBallot(ballotId);
      }

      return isSingle ? View("BallotSingle", ballotModel) : View("BallotNormal", ballotModel);
    }

    public JsonResult SaveVote(int pid, int vid, int count, int invalid = 0)
    {
      return CurrentBallotModel.SaveVote(pid, vid, count, invalid);
    }

    public JsonResult DeleteVote(int vid)
    {
      return CurrentBallotModel.DeleteVote(vid);
    }

    public JsonResult NeedsReview(bool needs)
    {
      return CurrentBallotModel.SetNeedsReview(needs);
    }

    public JsonResult SwitchToBallot(int ballotId)
    {
      return CurrentBallotModel.SwitchToBallotJson(ballotId);
    }

    public JsonResult UpdateLocationStatus(int id, string status)
    {
      return new LocationModel().UpdateStatus(id, status);
    }

    //public JsonResult UpdateBallotStatus(string status)
    //{
    //  return new
    //           {
    //             Status = status,
    //             Updated = false
    //           }.AsJsonResult();
    //  //return CurrentBallotModel.UpdateBallotStatus(status);
    //}
    
    public JsonResult UpdateLocationInfo(string info)
    {
      return new LocationModel().UpdateContactInfo(info);
    }
    
    public JsonResult UpdateLocationCollected(int numCollected)
    {
      return new LocationModel().UpdateNumCollected(numCollected);
    }

    public JsonResult RefreshBallotsList()
    {
      return CurrentBallotModel.CurrentBallotsInfoList().AsJsonResult();
    }

    public JsonResult SortVotes(string idList)
    {
      var ids = idList.Split(new[] {','}).Select(s => s.AsInt()).ToList();
      return CurrentBallotModel.SortVotes(ids).AsJsonResult();
    }

    public JsonResult NewBallot()
    {
      return CurrentBallotModel.StartNewBallotJson();
    }

    public JsonResult DeleteBallot()
    {
      return CurrentBallotModel.DeleteBallotJson();
    }

    private static IBallotModel CurrentBallotModel
    {
      get { return BallotModelFactory.GetForCurrentElection(); }
    }
  }
}