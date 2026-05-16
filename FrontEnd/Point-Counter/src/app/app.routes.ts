import { Routes } from '@angular/router';

import { Home } from './components/pages/home/home';
import { MatchPage } from './components/pages/match-page/match-page';
import { NotFound } from './components/pages/not-found/not-found';

export const routes: Routes = [
  {
    path: '',
    component: Home
  },
  {
    path: 'spel/:publicId',
    component: MatchPage
  },
  {
    path: '404',
    component: NotFound
  },
  {
    path: '**',
    component: NotFound
  }
];