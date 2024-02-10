import express from 'express';
import * as UserController from '../controllers/user-controller';
import CheckAuth from '../middleware/auth-handler';

const router = express.Router();

// Authentication and User controller end points
router.post('/register', UserController.create_user);
router.post('/signin', UserController.signin_user);
router.get('/:id', CheckAuth, UserController.get_user);
router.get('/logged/user', CheckAuth, UserController.get_logged_user);
// router.put('/user/update/:id', CheckAuth, UserController.update_user);
// router.put('/user/update/password/:id', CheckAuth, UserController.update_user_password);
// router.put('/user/check_unique_email', CheckAuth, UserController.check_unique_email);

// router.put('/user/password-reset/:id', CheckAuth, UserController.first_time_password_reset);

export default router;
