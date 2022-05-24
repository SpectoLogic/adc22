using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace adc22config.Models;
public class Settings
{
    public string BackgroundColor { get; set; } = string.Empty;
    public long FontSize { get; set; }
    public string FontColor { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string MyLittleSecret { get; set; } = string.Empty;
    public string MediaEncoderConfig { get; set; } = string.Empty;
}
