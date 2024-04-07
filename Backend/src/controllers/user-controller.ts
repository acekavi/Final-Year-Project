import express, { Request, Response, NextFunction } from 'express';
import mongoose from 'mongoose';
import bcrypt from 'bcrypt';
import jwt, { JwtPayload, VerifyErrors } from 'jsonwebtoken';

import User, { IUser } from '../models/user'; // Assume this is a TypeScript module or has .d.ts type declarations

const secretKey: string = process.env.JWT_SECRET || '';
interface DecodedToken extends JwtPayload {
  email: string;
  userId: string;
  iat: number;
  exp: number;
}

interface RequestWithUser extends Request {
  user?: DecodedToken;
}

// User controller functions
export const create_user = async (
  req: RequestWithUser,
  res: Response,
  next: NextFunction
): Promise<void> => {
  User.find({ email: req.body.email })
    .exec()
    .then((user) => {
      if (user.length >= 1) {
        return res.status(409).json({
          message: 'An account with the requested Email already exists.',
        });
      } else {
        bcrypt.hash(req.body.password, 10, (err, hash) => {
          if (err) {
            return res.status(500).json({
              error: err,
            });
          } else {
            const newUser = new User({
              _id: new mongoose.Types.ObjectId(),
              email: req.body.email,
              name: req.body.name,
              age: req.body.age,
              password: hash,
            });
            newUser
              .save()
              .then(async (result) => {
                console.log(
                  `User with email ${req.body.email} successfully created in the DB.`
                );
                // Assuming additional logic for email sending and other operations goes here
                res.json({
                  message: 'User created successfully.',
                });
              })
              .catch((err) => {
                console.log(err);
                res.status(500).json({
                  error: err,
                });
              });
          }
        });
      }
    });
};

export const signin_user = async (
  req: RequestWithUser,
  res: Response,
  next: NextFunction
): Promise<void> => {
  // Implementation for user sign-in
  User.findOne({ email: req.body.email })
    .exec()
    .then((user) => {
      if (!user) {
        return res.status(401).json({
          message: 'Authentication failed.',
        });
      }
      bcrypt.compare(req.body.password, user.password, (err, result) => {
        if (err) {
          return res.status(401).json({
            message: 'Authentication failed',
          });
        }
        if (result) {
          const token = jwt.sign(
            {
              email: user.email,
              userId: user._id,
            },
            secretKey,
            {
              expiresIn: '4h',
            }
          );
          return res.status(200).json({
            message: 'Authentication successful',
            token: token,
          });
        }
        res.status(401).json({
          message: 'Authentication failed',
        });
      });
    })
    .catch((err) => {
      console.log(err);
      res.status(500).json({
        error: err,
      });
    });
};

export const get_user = (
  req: RequestWithUser,
  res: Response,
  next: NextFunction
): void => {
  User.findById(req.user?.userId) // Assuming id is a string
    .select('-password -_id -pass_reset_required -notifications')
    .then((result: IUser | null) => {
      if (result) {
        res.send(result);
      } else {
        res.status(404).send('User not found');
      }
    })
    .catch((err: Error) => {
      console.error(err);
      res.status(400).send('Something went wrong: ' + err.message);
    });
};

export const get_logged_user = (
  req: RequestWithUser,
  res: Response,
  next: NextFunction
): Response | void => {
  try {
    const token = req.headers.authorization;
    if (!token) {
      return res.status(401).json({ message: 'No token provided.' });
    }

    const tokenValue = token.replace('Bearer ', '');
    const decoded = jwt.decode(tokenValue);
    if (!decoded) {
      return res.status(401).json({ message: 'Invalid token.' });
    }
    // Verify and decode the JWT
    jwt.verify(tokenValue, (secretKey) => {
      const decodedToken = decoded as DecodedToken; // Type assertion for decoded token
      const userId = decodedToken.userId;

      User.findById(userId) // Filters the user by Id
        .select('-password -_id')
        .then((result) => {
          if (!result) {
            return res.status(404).json({ message: 'User not found.' });
          }
          res.send(result);
        })
        .catch((err) => {
          console.error(err);
          res.status(400).send('Something went wrong: ' + err.message);
        });
    });
  } catch (error) {
    console.error(error);
    res.status(500).json({ message: 'Error getting logged user details.' });
  }
};

export const add_achievement = (
  req: RequestWithUser,
  res: Response,
  next: NextFunction
): void => {
  try {
    const userId = req.user?.userId;
    const achievement = req.body.achievement;
    User.findById(userId)
      .then((user) => {
        if (!user) {
          return res.status(404).json({ message: 'User not found.' });
        }
        if (!user.achievements.includes(achievement)) {
          user.achievements.push(achievement);
        }
        user.save().then(() => {
          res.json({ message: 'Achievement added successfully.' });
        }).catch((err) => {
          console.error(err);
          res.status(500).json({ message: 'Error adding achievement.' });
        });
      }).catch((err) => {
        console.error(err);
        res.status(500).json({ message: 'Error adding achievement.' });
      });
  } catch (error) {
    console.error(error);
    res.status(500).json({ message: 'Error adding achievement.' });
  }
};

export const add_badge = (
  req: RequestWithUser,
  res: Response,
  next: NextFunction
): void => {
  try {
    const userId = req.user?.userId;
    const badge = req.body.badge;
    User.findById(userId)
      .then((user) => {
        if (!user) {
          return res.status(404).json({ message: 'User not found.' });
        }
        if (!user.badges.includes(badge)) {
          user.badges.push(badge);
        }
        user.save().then(() => {
          res.json({ message: 'Badge added successfully.' });
        }).catch((err) => {
          console.error(err);
          res.status(500).json({ message: 'Error adding Badge.' });
        });
      }).catch((err) => {
        console.error(err);
        res.status(500).json({ message: 'Error adding Badge.' });
      });
  } catch (error) {
    console.error(error);
    res.status(500).json({ message: 'Error adding Badge.' });
  }
};

export const get_auth_status = (
  req: RequestWithUser,
  res: Response,
  next: NextFunction
): void => {
  res.status(200).json({
    message: 'User is authenticated.',
    auth: true,
  });
};
