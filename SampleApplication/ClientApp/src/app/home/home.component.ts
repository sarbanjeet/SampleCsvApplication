import {AfterViewInit, Component, OnInit, ViewChild} from '@angular/core';
import {ConsumptionForecastService} from './consumption-forecast.service';
import {MatTableDataSource} from '@angular/material/table';
import {MatSort, Sort} from '@angular/material/sort';
import {MatPaginator} from '@angular/material/paginator';
import Chart from 'chart.js/auto'
import { MatSelectChange } from '@angular/material/select';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})


export class HomeComponent implements OnInit, AfterViewInit {
  statistic: any;
  chartHours: number = 0;
  displayedColumns: string[] = ['dateTime', 'amount'];
  chart: any;
  transformedData: DataItem[] = [];
  dataSource = new MatTableDataSource<DataItem>([]);
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  private chartData: any;

  constructor(private consumptionForecastService: ConsumptionForecastService) {
  }

  ngOnInit(): void {
    this.consumptionForecastService.getStatistics().subscribe(
      (data) => {
        this.statistic = data;
        this.dataSource.data = data.data;
        this.chartData = data.data;
        this.GetByDayData(this.chartData, 0);
      },
      (error) => {
        console.error('There was an error!', error);
      }
    );
  }


  public onChangeDrawChart(event: MatSelectChange) {
    if (this.chart) {
      this.chart.destroy();
    }

    let dataFeedByHalfHour = 2;
    this.GetByDayData(this.chartData, Number(this.chartHours) * dataFeedByHalfHour);
  }

  private GetByDayData(originalData: DataItem[], hours: number) {
    this.transformedData = [];

    if (hours <= 0) {
      this.transformedData = [...originalData];
      this.DrawChart(this.transformedData);
      return;
    }

    for (let i = 0; i < originalData.length; i += hours) {
      let amount = 0;
      let length = Math.min(i + hours, originalData.length);

      for (let j = i; j < length; j++) {
        amount += originalData[j].amount;
      }


      this.transformedData.push({
        dateTime: originalData[i].dateTime,
        amount: amount
      });
    }

    this.DrawChart(this.transformedData);
  }

  private DrawChart(data: DataItem[]) {
    this.chart = new Chart("MyChart", {
      type: 'line',
      data: {
        labels: data.map((i: DataItem) => new Date(i.dateTime).toDateString()),
        datasets: [
          {
            label: "Amount by provided hour(s)",
            data: data.map((i: DataItem) => i.amount),
            backgroundColor: "blue"
          }
        ]
      },
      options: {
        aspectRatio: 3,
      }

    });
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }
}

interface DataItem {
  dateTime: string;
  amount: number;
}
