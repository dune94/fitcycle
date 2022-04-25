import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { Polylines } from "./polylines";
import { Laps } from "./laps";
import { Load } from "./load";

@Injectable({
  providedIn: 'root'
})

export class AppService {

    constructor(private http: HttpClient) { }

    headers = new HttpHeaders().set('Content-Type', 'application/json');

    public getPolylinesLap(lapId: string): Observable<Polylines[]> {
        let httpParams = new HttpParams()
                        .set('lapid', lapId)
        return this.http.get<Polylines[]>('api/httptriggerqueryfitbitpolylinesapi', {params: httpParams});
    }

    public getLaps(): Observable<Laps[]> {
        return this.http.get<Laps[]>('api/httptriggerqueryfitbitlapsapi');
    }

    public loadData(fitbitCode: string): Observable<Load> {
        let httpParams = new HttpParams()
                        .set('fitbit', fitbitCode)
        return this.http.get<Load>('api/httptriggerfitbitqueryloadapi', {params: httpParams});
    }

    error(error: HttpErrorResponse) {
        let errorMessage = '';
        if (error.error instanceof ErrorEvent) {
            errorMessage = error.error.message;
        } else {
            errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
        }
        console.log(errorMessage);
        return throwError(errorMessage);
    }

}