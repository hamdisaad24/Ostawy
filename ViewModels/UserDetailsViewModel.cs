using Ostawy.Models;

namespace Ostawy.ViewModels;

public class UserDetailsViewModel
{
    public ApplicationUser User { get; set; } = null!;

    public int RequestsCount { get; set; }

    public int BidsCount { get; set; }

    public int AcceptedBidsCount { get; set; }

    public bool IsPro { get; set; }

    public List<JobRequest> Requests { get; set; } = new();

    public List<JobBid> Bids { get; set; } = new();
}