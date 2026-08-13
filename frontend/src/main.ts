import { Component, OnInit } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { HttpClient, provideHttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

type Todo = { id: number; title: string; complete: boolean };

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule],
  template: `
    <main>
      <header><span class="status"></span><strong>Company Staging</strong><small>Integration environment</small></header>
      <section>
        <p class="label">SYSTEM STATUS</p>
        <h1>{{ error ? 'Service unavailable' : 'Environment operational' }}</h1>
        <p>{{ error || 'Angular, ASP.NET Core, and SQL Server are connected.' }}</p>
        <div class="checks" *ngFor="let todo of todos"><span>API / Database</span><b>{{ todo.title }}</b></div>
      </section>
    </main>`,
})
class App implements OnInit {
  todos: Todo[] = [];
  error = '';
  constructor(private http: HttpClient) {}
  ngOnInit() { this.http.get<Todo[]>('/api/todos').subscribe({ next: x => this.todos = x, error: () => this.error = 'The API health check failed. Review container diagnostics.' }); }
}

bootstrapApplication(App, { providers: [provideHttpClient()] });

