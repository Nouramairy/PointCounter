import { Routes } from '@angular/router';

import { Home } from './components/pages/home/home';
import { MatchPage } from './components/pages/match-page/match-page';
import { NotFound } from './components/pages/not-found/not-found';
import { Manage } from './components/pages/manage/manage';
import { Players } from './components/pages/players/players';
import { Teams } from './components/pages/teams/teams';
import { Games } from './components/pages/games/games';
import { PoangstallningHub } from './components/pages/poangstallning/poangstallning-hub';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'quick-match',
    pathMatch: 'full'
  },
  {
    path: 'quick-match',
    component: Home
  },
  {
    path: 'match/:publicId',
    component: MatchPage
  },
  {
    path: 'spel/:publicId',
    redirectTo: 'match/:publicId'
  },
  {
    path: 'create-match',
    component: Manage
  },
  {
    path: 'create-match/players',
    component: Players
  },
  {
    path: 'create-match/teams',
    component: Teams
  },
  {
    path: 'create-match/games',
    component: Games
  },
  {
    path: 'create-match/scoreboard',
    component: PoangstallningHub
  },
  {
    path: 'manage/players',
    redirectTo: 'create-match/players'
  },
  {
    path: 'manage/teams',
    redirectTo: 'create-match/teams'
  },
  {
    path: 'manage/games',
    redirectTo: 'create-match/games'
  },
  {
    path: 'manage/scoreboard',
    redirectTo: 'create-match/scoreboard'
  },
  {
    path: 'manage/poangstallning',
    redirectTo: 'create-match/scoreboard'
  },
  {
    path: 'manage',
    redirectTo: 'create-match',
    pathMatch: 'full'
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
