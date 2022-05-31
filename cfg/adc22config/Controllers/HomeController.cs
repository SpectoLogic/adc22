using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using adc22config.Models;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement.Mvc;
using Microsoft.FeatureManagement;

namespace adc22config.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IFeatureManager _featureManager;

    public HomeController(
        ILogger<HomeController> logger,
        IOptionsSnapshot<Settings> settings,
        IFeatureManager featureManager
        )

    {
        _logger = logger;
        _featureManager = featureManager;
    }

    public async Task<IActionResult> Index()
    {
        if (await _featureManager.IsEnabledAsync(AppFeatureFlags.FeatureA))
        {

        }
        return View();
    }

    [FeatureGate(AppFeatureFlags.FeatureA)]
    public IActionResult FeatureA()
    {
        return View();
    }

    [FeatureGate(AppFeatureFlags.FeatureB)]
    public IActionResult FeatureB()
    {
        return View();
    }
}
