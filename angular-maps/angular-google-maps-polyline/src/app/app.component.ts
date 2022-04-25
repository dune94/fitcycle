import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppService } from "./app.service"
import { Polylines } from "./polylines";
import { Laps } from "./laps";
import { Path } from "./path";
import { Load } from "./load";
import { environment } from '../environments/environment';
import { ChartType } from "angular-google-charts";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: [ './app.component.css' ]
})
export class AppComponent  {

  constructor(public appService: AppService, private route: ActivatedRoute, private router: Router) { }
  
  // google maps zoom level
  // TODO base this on an API call
  zoom: number = environment.zoomLevel;

  polylines!: Polylines[];
  paths!: Path[];
  laps!: Laps[];
  fitbitCode!: string;
  selectedLap!: Laps;
  load!: Load;
  ingesting!: boolean;
  flex!: string;

  calories!: any[];
  distances!: any[];
  durations!: any[];
  fastests!: any[];
  fastestkms!: any[];
  highesthrs!: any[];
  averagehrs!: any[];

  // initial center position for the map
  // TODO base this on an API call
  latitude: number = environment.initialLatitude;
  longitude: number = environment.initialLogitude;
  
  distcollapsed!: boolean;
  calcollapsed!: boolean;
  durcollapsed!: boolean;
  fastcollapsed!: boolean;
  fastkmcollapsed!: boolean;
  highhrcollapsed!: boolean;
  avghrcollapsed!: boolean;

  showCalories!: boolean;
  showDistance!: boolean;
  showDuration!: boolean;
  showFastest!: boolean;
  showFastestKm!: boolean;
  showHighestHr!: boolean;
  showAverageHr!: boolean;

  caloryChart = {
    title: 'Calories',
    type: ChartType.ColumnChart,
    data: this.calories,
    options: {
      colors: ['#5EB2B3'],
      chartArea: {width: '50%'},
      isStacked: true,
      backgroundColor: {
        fill:'#FFF030',
        fillOpacity: 0.8     
      },
      animation: {
        startup : true,
        duration: 1000,
        easing: 'out'
      },
    },
    width: 1000,
    height: 500
  };

  distanceChart = {
    title: 'Distance',
    type: ChartType.ColumnChart,
    data: this.distances,
    options: {
      colors: ['#5EB2B3'],
      chartArea: {width: '50%'},
      isStacked: true,
      backgroundColor: {
        fill:'#FFF030',
        fillOpacity: 0.8     
      },
      animation: {
        startup : true,
        duration: 1000,
        easing: 'out'
      },
    },
    width: 1000,
    height: 500
  };

  durationChart = {
    title: 'Duration',
    type: ChartType.ColumnChart,
    data: this.durations,
    options: {
      colors: ['#5EB2B3'],
      chartArea: {width: '50%'},
      isStacked: true,
      backgroundColor: {
        fill:'#FFF030',
        fillOpacity: 0.8     
      },
      animation: {
        startup : true,
        duration: 1000,
        easing: 'out'
      },
    },
    width: 1000,
    height: 500
  };

  fastestChart = {
    title: 'Fastest',
    type: ChartType.ColumnChart,
    data: this.fastests,
    options: {
      colors: ['#5EB2B3'],
      chartArea: {width: '50%'},
      isStacked: true,
      backgroundColor: {
        fill:'#FFF030',
        fillOpacity: 0.8     
      },
      animation: {
        startup : true,
        duration: 1000,
        easing: 'out'
      },
    },
    width: 1000,
    height: 500
  };

  fastestkmChart = {
    title: 'FastestKm',
    type: ChartType.ColumnChart,
    data: this.fastestkms,
    options: {
      colors: ['#5EB2B3'],
      chartArea: {width: '50%'},
      isStacked: true,
      backgroundColor: {
        fill:'#FFF030',
        fillOpacity: 0.8     
      },
      animation: {
        startup : true,
        duration: 1000,
        easing: 'out'
      },
    },
    width: 1000,
    height: 500
  };

  highesthrChart = {
    title: 'HighestHr',
    type: ChartType.ColumnChart,
    data: this.highesthrs,
    options: {
      colors: ['#5EB2B3'],
      chartArea: {width: '50%'},
      isStacked: true,
      backgroundColor: {
        fill:'#FFF030',
        fillOpacity: 0.8     
      },
      animation: {
        startup : true,
        duration: 1000,
        easing: 'out'
      },
    },
    width: 1000,
    height: 500
  };

  averagehrChart = {
    title: 'AverageHr',
    type: ChartType.ColumnChart,
    data: this.averagehrs,
    options: {
      colors: ['#5EB2B3'],
      chartArea: {width: '50%'},
      isStacked: true,
      backgroundColor: {
        fill:'#FFF030',
        fillOpacity: 0.8     
      },
      animation: {
        startup : true,
        duration: 1000,
        easing: 'out'
      },
    },
    width: 1000,
    height: 500
  };

  ngOnInit() {

    this.ingesting = false;
    this.flex = '14%';

    this.calcollapsed = true;
    this.distcollapsed = true;
    this.durcollapsed = true;
    this.fastcollapsed = true;
    this.fastkmcollapsed = true;
    this.highhrcollapsed = true;
    this.avghrcollapsed = true;

    this.showCalories = true;
    this.showDistance = true;
    this.showDuration = true;
    this.showFastest = true;
    this.showFastestKm = true;
    this.showHighestHr = true;
    this.showAverageHr = true;

    this.route.queryParams
      .subscribe(params => {
        console.log(params); 
        this.fitbitCode = params.code;
        if (this.fitbitCode != null){
          this.appService.loadData(this.fitbitCode).subscribe((data: Load) => {
            this.load = data;
            if (this.load.DataExists == "yes"){
              this.ingesting = true;
            }
          });
          this.appService.getLaps().subscribe((data: Laps[]) => {
            this.laps = data;
            this.calories = this.laps.map(lap=>{
              return [lap.StartTime, Number(lap.Calories)];
            });
            this.caloryChart.data = this.calories;
            this.distances = this.laps.map(lap=>{
              return [lap.StartTime, Number(lap.DistanceKms)];
            });
            this.distanceChart.data = this.distances;
            this.durations = this.laps.map(lap=>{
              return [lap.StartTime, Number(lap.TotalTimeMinutes)];
            });
            this.durationChart.data = this.durations;
            this.fastests = this.laps.map(lap=>{
              return [lap.StartTime, Number(lap.FastestSpeed)];
            });
            this.fastestChart.data = this.fastests;
            this.fastestkms = this.laps.map(lap=>{
              return [lap.StartTime, Number(lap.FastestKm)];
            });
            this.fastestkmChart.data = this.fastestkms;
            this.highesthrs = this.laps.map(lap=>{
              return [lap.StartTime, Number(lap.HighestHr)];
            });
            this.highesthrChart.data = this.highesthrs;
            this.averagehrs = this.laps.map(lap=>{
              return [lap.StartTime, Number(lap.AverageHr)];
            });
            this.averagehrChart.data = this.averagehrs;
          });
        }
      }
    );
  }

  public onLapsSelected() {
    this.appService.getPolylinesLap(this.selectedLap.id).subscribe((data: Polylines[]) => {
      this.polylines = data;
      for(var i = 0; i < this.polylines.length; i++)
      { 
        this.paths = this.polylines[i].path;
      }
    });
  }

  public caloriesShow() {
    this.flex = '100%';
    this.calcollapsed = false;
    this.showDuration = false;
    this.showDistance = false;
    this.showFastest = false;
    this.showFastestKm = false;
    this.showHighestHr = false;
    this.showAverageHr = false;
  }

  public caloriesHide() {
    this.flex = '14%';
    this.calcollapsed = true;
    this.showDuration = true;
    this.showDistance = true;
    this.showFastest = true;
    this.showFastestKm = true;
    this.showHighestHr = true;
    this.showAverageHr = true;
  }

  public distanceShow() {
    this.flex = '100%';
    this.distcollapsed = false;
    this.showDuration = false;
    this.showCalories = false;
    this.showFastest = false;
    this.showFastestKm = false;
    this.showHighestHr = false;
    this.showAverageHr = false;
  }

  public distanceHide() {
    this.flex = '14%';
    this.distcollapsed = true;
    this.showDuration = true;
    this.showCalories = true;
    this.showFastest = true;
    this.showFastestKm = true;
    this.showHighestHr = true;
    this.showAverageHr = true;
  }

  public durationShow() {
    this.flex = '100%';
    this.durcollapsed = false;
    this.showCalories = false;
    this.showDistance = false;
    this.showFastest = false;
    this.showFastestKm = false;
    this.showHighestHr = false;
    this.showAverageHr = false;
  }

  public durationHide() {
    this.flex = '14%';
    this.durcollapsed = true;
    this.showCalories = true;
    this.showDistance = true;
    this.showFastest = true;
    this.showFastestKm = true;
    this.showHighestHr = true;
    this.showAverageHr = true;
  }
  
  public fastestShow() {
    this.flex = '100%';
    this.fastcollapsed = false;
    this.showDuration = false;
    this.showCalories = false;
    this.showDistance = false;
    this.showFastestKm = false;
    this.showHighestHr = false;
    this.showAverageHr = false;
  }

  public fastestHide() {
    this.flex = '14%';
    this.fastcollapsed = true;
    this.showDuration = true;
    this.showCalories = true;
    this.showDistance = true;
    this.showFastestKm = true;
    this.showHighestHr = true;
    this.showAverageHr = true;
  }

  public fastestKmShow() {
    this.flex = '100%';
    this.fastkmcollapsed = false;
    this.showDuration = false;
    this.showCalories = false;
    this.showDistance = false;
    this.showFastest = false;
    this.showHighestHr = false;
    this.showAverageHr = false;
  }

  public fastestKmHide() {
    this.flex = '14%';
    this.fastkmcollapsed = true;
    this.showDuration = true;
    this.showCalories = true;
    this.showDistance = true;
    this.showFastest = true;
    this.showHighestHr = true;
    this.showAverageHr = true;
  }

  public highestHrShow() {
    this.flex = '100%';
    this.highhrcollapsed = false;
    this.showDuration = false;
    this.showCalories = false;
    this.showDistance = false;
    this.showFastestKm = false;
    this.showFastest = false;
    this.showAverageHr = false;
  }

  public highestHrHide() {
    this.flex = '14%';
    this.highhrcollapsed = true;
    this.showDuration = true;
    this.showCalories = true;
    this.showDistance = true;
    this.showFastestKm = true;
    this.showFastest = true;
    this.showAverageHr = true;
  }

  public averageHrShow() {
    this.flex = '100%';
    this.avghrcollapsed = false;
    this.showDuration = false;
    this.showCalories = false;
    this.showDistance = false;
    this.showFastestKm = false;
    this.showFastest = false;
    this.showHighestHr = false;
  }

  public averageHrHide() {
    this.flex = '14%';
    this.avghrcollapsed = true;
    this.showDuration = true;
    this.showCalories = true;
    this.showDistance = true;
    this.showFastestKm = true;
    this.showFastest = true;
    this.showHighestHr = true;
  }

}

// just an interface for type safety.
interface polylinesObject {
  Id: string;
  path: Path[];
  color: string;
}
